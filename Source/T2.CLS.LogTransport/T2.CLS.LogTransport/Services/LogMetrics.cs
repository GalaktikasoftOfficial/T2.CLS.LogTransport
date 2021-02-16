using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Timers;
using T2.Cls.LogTransport.Common.Interfaces;
using T2.CLS.LogTransport.Extensions;
using Timer = System.Timers.Timer;

namespace T2.CLS.LoggerExtensions.Clickhouse.Services
{
	internal sealed class LogMetrics : ILogMetrics
	{
		#region Fields

		private bool _dirty;
		private int _handled;

		private Timer _timer;
		private int _written;
		private readonly ILogger _logger;

		#endregion

		#region Ctors

		public LogMetrics(ILoggerFactory loggerFactory)
		{

			_logger = loggerFactory.CreateLogger<LogMetrics>();

			_logger.SLT00008_Debug_LogMetrics_Start();

			_timer = new Timer(1000);
			_timer.Elapsed += TimerOnElapsed;
			_timer.Start();
            _logger.SLT00009_Debug_LogMetrics_End();
		}

		#endregion

		#region Methods

		private void TimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			WriteInfo();
		}

		private void WriteInfo()
		{
			if (_dirty == false)
				return;

			_dirty = false;
		}

		#endregion

		#region Interface Implementations

		#region ILogMetrics

		public void LogEntriesWritten(int count)
		{
			_dirty = true;

			Interlocked.Add(ref _written, count);
		}

		public void LogEntriesHandled(int count)
		{
			_dirty = true;

			Interlocked.Add(ref _handled, count);
		}

		#endregion

		#endregion
	}
}