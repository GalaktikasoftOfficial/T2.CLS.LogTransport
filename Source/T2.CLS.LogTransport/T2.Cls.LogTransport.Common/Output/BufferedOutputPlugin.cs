// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using T2.Cls.LogTransport.Common.Interfaces;
using T2.CLS.LoggerExtensions.Core.Buffer;
using T2.CLS.LoggerExtensions.Core.Config;
using T2.Cls.LogTransport.Common.Extensions;

namespace T2.Cls.LogTransport.Common.Output
{
	public abstract class BufferedOutputPlugin : GenericFileBuffer<string>, IOutputPlugin
	{
		#region Fields

		private readonly ILogMetrics _metrics;
		private ILogger _logger;

		#endregion

		#region Ctors

		protected BufferedOutputPlugin(string systemName, ILogMetrics metrics) : base(Path.Combine("LogBuffer", systemName),
			4 * 1024, 4 * 256, 1024 * 16 * 4, 1000, workerCount: 4)
		{
			_logger = NullLogger.Instance;
            _logger.SLT00015_Trace_BufferedOutputPlugin_string_systemName_ILogMetrics_metrics();
			_metrics = metrics;
		}

		protected BufferedOutputPlugin(
			string systemName,
			ILogMetrics metrics,
			string path,
			int readLimit = 4 * 1024,
			int workerCount = 4,
			int? memoryBufferLimit = 512,
			int? fileBufferLimit = 512 * 16,
			double? flushTimeout = 3000,
			double? resendTimeout = 5000,
			double[] resendIntervals =  null,
			Encoding encoding = null,
			ILoggerFactory loggerFactory = null
			)
			: base(Path.Combine(path, systemName), 
				readLimit : readLimit,
				workerCount : workerCount, 
				memoryBufferLimit: memoryBufferLimit, 
				fileBufferLimit: fileBufferLimit, 
				flushTimeout: flushTimeout,
				resendTimeout: resendTimeout,
				resendIntervals: resendIntervals,
				encoding:null,
				loggerFactory:null)
		{
			_logger = loggerFactory?.CreateLogger(typeof(BufferedOutputPlugin)) ?? NullLogger.Instance;
            _logger.SLT00016_Trace_BufferedOutputPlugin();
			_metrics = metrics;
		}

		protected BufferedOutputPlugin(
			string systemName,
			ILogMetrics metrics,
			BufferSettingsConfig settingsConfig = null,
			ILoggerFactory loggerFactory = null) 
			: base(Path.Combine(settingsConfig?.BufferPath ?? "LogBuffer", systemName),
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
			_logger = loggerFactory?.CreateLogger(typeof(BufferedOutputPlugin)) ?? NullLogger.Instance;
            _logger.SLT00017_Trace_BufferedOutputPlugin_BufferSettingsConfig_config(settingsConfig);
			_metrics = metrics;
		}


		#endregion

		#region Methods

		protected override void FormatItem(string item, StreamWriter streamWriter)
		{
			streamWriter.Write(item);
		}

		protected override void HandleRead(IReadOnlyList<string> buffer)
		{
			PushBuffer(buffer);
		}

		protected abstract void PushBuffer(IReadOnlyList<string> buffer);

		#endregion

		#region Interface Implementations

		#region IOutputPlugin

		public async ValueTask<long> WriteLogAsync(StreamReader streamReader)
		{
			var content = await streamReader.ReadToEndAsync();
			using var stringReader = new StringReader(content);
			var lineCount = 0;
			string line;

			while ((line = stringReader.ReadLine()) != null)
			{
				Write(line);
				lineCount++;
			}

			_metrics.LogEntriesWritten(lineCount);

			return lineCount;
		}


		public long WriteLog(string content)
		{
			using var stringReader = new StringReader(content);
			var lineCount = 0;
			string line;

			while ((line = stringReader.ReadLine()) != null)
			{
				Write(line);
				lineCount++;
			}

			_metrics.LogEntriesWritten(lineCount);

			return lineCount;
		}
		public long PendingCount => GetPendingLineCount();

		public double SendRatePerSecond => GetSendRatePerSecond();

		#endregion

		#endregion
	}
}