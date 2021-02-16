// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using T2.Cls.LogTransport.Common.Interfaces;
using T2.Cls.LogTransport.Common.Structs;
using T2.Cls.LogTransport.OutputPlugin.Forward.Config;
using T2.CLS.LoggerExtensions.Core.Buffer;
using T2.CLS.LoggerExtensions.Core.Config;
using Timer = System.Timers.Timer;

namespace T2.Cls.LogTransport.OutputPlugin.Forward.Output
{
	public sealed class ForwardOutputPlugin : IOutputPlugin
	{
		#region Properties

		private string System => _config.System;
		private long _totalSend;
		private bool _updating;
		private readonly ILogger _logger;

		#endregion

		#region Ctors

		public ForwardOutputPlugin(ForwardOutputConfig config, ILoggerFactory loggerFactory)
		{
			_config = config;
			_fileBuffer = new FileBuffer(this, config.BufferSettings, loggerFactory);
			_updateServersTimer = new Timer
			{
				AutoReset = true,
				Interval = 10000,
				Enabled = true
			};

			_updateServersTimer.Elapsed += OnUpdateServersTimerElapsed;
			_logger = loggerFactory.CreateLogger(typeof(ForwardOutputPlugin));

			foreach (var server in config.Servers)
				_workers.Add(new ForwardWorker(this, server));

			UpdateServers();
		}

		private void OnUpdateServersTimerElapsed(object sender, ElapsedEventArgs e)
		{
			if (_updating)
				return;

			try
			{
				_updateServersTimer.Stop();

				UpdateServers();
			}
			finally
			{
				_updateServersTimer.Start();
			}
		}

		private void UpdateServers()
		{
			try
			{
				_updating = true;

				var updateTasks = new Task[_workers.Count];

				for (var index = 0; index < _workers.Count; index++)
					updateTasks[index] = _workers[index].UpdatePending();

				Task.WaitAll(updateTasks.ToArray());
			}
			finally
			{
				_updating = false;
			}
		}

		#endregion

		#region Fields

		private readonly ForwardOutputConfig _config;
		private readonly FileBuffer _fileBuffer;
		private readonly HttpClient _httpClient = new HttpClient();
		private readonly List<ForwardWorker> _workers = new List<ForwardWorker>();
		private readonly Timer _updateServersTimer;

		#endregion

		#region Interface Implementations

		#region IOutputPlugin

		public async ValueTask<long> WriteLogAsync(StreamReader streamReader)
		{
			var content = await streamReader.ReadToEndAsync().ConfigureAwait(false);
			var result = await HandleBufferBlock(content);

			return result < 0 ? SaveBuffer(content) : result;
		}

		public long WriteLog(string content)
		{
			var result = HandleBufferBlock(content).Result;

			return result < 0 ? SaveBuffer(content) : result;
		}


		private async ValueTask<long> HandleBufferBlock(string content)
		{
			for (var i = 0; i < 3; i++)
			{
				var topWorker = GetTopWorker();

				if (topWorker == null)
					break;

				var result = await topWorker.PushBuffer(content);

				if (result >= 0)
				{
					_totalSend += result;

					return result;
				}
			}

			return -1;
		}

		private ForwardWorker GetTopWorker()
		{
			var sw = new SpinWait();

			while (_updating)
				sw.SpinOnce();

			return _workers
				.Where(w => w.IsBanned == false)
				.OrderBy(w => Math.Ceiling(w.Pending / w.SendRatePerSecond))
				.ThenBy(w => w.TotalSend).FirstOrDefault();
		}

		private long SaveBuffer(string content)
		{

			using var stringReader = new StringReader(content);

			string line;
			long count = 0;

			while ((line = stringReader.ReadLine()) != null)
			{
				_fileBuffer.Write(line);
				count++;
			}

			return count;
		}

		public long PendingCount => 0;

		public double SendRatePerSecond => 100000;

		#endregion

		#endregion

		#region Nested Types

		private sealed class FileBuffer : GenericFileBuffer<string>
		{
			#region Fields

			private readonly ForwardOutputPlugin _forwarder;

			#endregion

			#region Ctors

			public FileBuffer(ForwardOutputPlugin forwarder) : base(Path.Combine("LogBuffer", "Forward", forwarder.System),
				4 * 1024, 4 * 256, 1024 * 16 * 4, 1000)
			{
				_forwarder = forwarder;
			}

			public FileBuffer(ForwardOutputPlugin forwarder,
							BufferSettingsConfig settingsConfig = null,
							ILoggerFactory loggerFactory = null)
				: base(Path.Combine(settingsConfig?.BufferPath ?? "LogBuffer", forwarder.System),
					settingsConfig?.ReadLimit ?? 4 * 1024,
					settingsConfig?.MemoryBufferLimit,
					settingsConfig?.FileBufferLimit,
					settingsConfig?.FlushTimeout,
					settingsConfig?.ResendTimeout,
					settingsConfig?.ResendIntervals,
					settingsConfig?.WorkerCount ?? 1,
					settingsConfig?.Encoding,
					loggerFactory)
			{
				_forwarder = forwarder;
			}

			#endregion

			#region Methods

			protected override void FormatItem(string item, StreamWriter streamWriter)
			{
				streamWriter.Write(item);
			}

			protected override void HandleRead(IReadOnlyList<string> buffer)
			{
				var content = string.Join(Environment.NewLine, buffer);
				var result = _forwarder.HandleBufferBlock(content).Result;

				if (result < 0)
					throw new InvalidOperationException();
			}

			#endregion
		}

		private sealed class ForwardWorker
		{
			private static readonly List<TimeSpan> BanTimeouts = new List<TimeSpan>
			{
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(10),
				TimeSpan.FromSeconds(10),
				TimeSpan.FromSeconds(10),
				TimeSpan.FromSeconds(20),
				TimeSpan.FromSeconds(20),
				TimeSpan.FromSeconds(30),
				TimeSpan.FromSeconds(60)
			};

			private readonly ForwardOutputPlugin _forwarder;
			private readonly Uri _getPendingUri;
			private readonly Uri _saveUri;
			private readonly string _server;
			private long _lastUpdatePending;
			private long _sendSinceLastUpdate;

			#region Ctors

			public ForwardWorker(ForwardOutputPlugin forwarder, string server)
			{
				_forwarder = forwarder;
				_server = server;

				var normalizedServer = server.TrimEnd('/');

				_saveUri = new Uri(normalizedServer + $"/Log/Save/{forwarder.System}");
				_getPendingUri = new Uri(normalizedServer + "/Log/Metrics");
			}

			#endregion

			public long TotalSend { get; private set; }

			public double SendRatePerSecond { get; private set; } = 50000;

			private int ContiguousExceptionsCount { get; set; }

			private DateTime BannedUntil { get; set; }

			public bool IsBanned => DateTime.Now < BannedUntil;

			#region Methods

			public async ValueTask<long> PushBuffer(string content)
			{
				if (IsBanned)
					return -1;

				try
				{
					using var stringContent = new StringContent(content);

					var result = await _forwarder._httpClient.PostAsync(_saveUri, stringContent);

					if (result.IsSuccessStatusCode)
					{
						var written = await result.Content.ReadAsAsync<long>();

						TotalSend += written;
						_sendSinceLastUpdate += written;
						ContiguousExceptionsCount = 0;

						return written;
					}
				}
				catch
				{
				}

				HandleSendError();

				return -1;
			}

			private void HandleSendError()
			{
				BannedUntil = DateTime.Now + (ContiguousExceptionsCount < BanTimeouts.Count
					              ? BanTimeouts[ContiguousExceptionsCount]
					              : BanTimeouts.Last() * Math.Pow(2, ContiguousExceptionsCount - BanTimeouts.Count));

				ContiguousExceptionsCount++;

				_forwarder.BanWorker(this);
			}

			public double Pending => _sendSinceLastUpdate + _lastUpdatePending;

			public async Task UpdatePending()
			{
				if (IsBanned)
					return;

				var cts = new CancellationTokenSource();

				cts.CancelAfter(TimeSpan.FromSeconds(10));

				try
				{
					var cancellationToken = cts.Token;
					var result = await _forwarder._httpClient.GetAsync(_getPendingUri, cancellationToken);
					var metricsData = await result.Content.ReadAsAsync<MetricsData>(cancellationToken);

					SendRatePerSecond = metricsData.SendRatePerSecond;
					_lastUpdatePending = metricsData.Pending;
					_sendSinceLastUpdate = 0;

					_forwarder._logger.LogInformation(
						$"Worker {_server} Pending: {Pending} Rate: {Math.Round(SendRatePerSecond)} p/s");
				}
				catch (OperationCanceledException)
				{
				}
				catch(HttpRequestException)
				{
					HandleSendError();
				}
				finally
				{
					cts.Dispose();
				}
			}

			#endregion
		}

		private void BanWorker(ForwardWorker forwardWorker)
		{
		}

		#endregion
	}
}