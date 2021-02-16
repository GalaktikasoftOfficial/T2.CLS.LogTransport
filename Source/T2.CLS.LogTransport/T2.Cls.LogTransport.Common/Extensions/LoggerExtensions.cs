using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using T2.Cls.LogTransport.Common.Properties;

namespace T2.Cls.LogTransport.Common.Extensions
{
    public static class LoggerExtensions
    {
		private static readonly Action<ILogger, Exception> _SLT00015_Trace_BufferedOutputPlugin_string_systemName_ILogMetrics_metrics;
		private static readonly Action<ILogger, Exception> _SLT00016_Trace_BufferedOutputPlugin;
		private static readonly Action<ILogger, object, Exception> _SLT00017_Trace_BufferedOutputPlugin_BufferSettingsConfig_config;

        static LoggerExtensions()
        {
            _SLT00015_Trace_BufferedOutputPlugin_string_systemName_ILogMetrics_metrics = LoggerMessage.Define(
                LogLevel.Trace,
                new EventId(15, nameof(Logs.SLT00015)),
                Logs.SLT00015);
            _SLT00016_Trace_BufferedOutputPlugin = LoggerMessage.Define(
                LogLevel.Trace,
                new EventId(16, nameof(Logs.SLT00016)),
                Logs.SLT00016);
            _SLT00017_Trace_BufferedOutputPlugin_BufferSettingsConfig_config = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(17, nameof(Logs.SLT00017)),
                Logs.SLT00017);
        }
        
        /// <summary>
        /// ~BufferedOutputPlugin(string systemName, ILogMetrics metrics)
        /// </summary>
        public static void SLT00015_Trace_BufferedOutputPlugin_string_systemName_ILogMetrics_metrics(this ILogger logger)
        {
            _SLT00015_Trace_BufferedOutputPlugin_string_systemName_ILogMetrics_metrics(logger, null);
        }

        /// <summary>
        /// ~BufferedOutputPlugin
        /// </summary>
        public static void SLT00016_Trace_BufferedOutputPlugin(this ILogger logger)
        {
            _SLT00016_Trace_BufferedOutputPlugin(logger, null);
        }

        /// <summary>
        /// ~BufferedOutputPlugin; BufferSettingsConfig:{config}
        /// </summary>
        public static void SLT00017_Trace_BufferedOutputPlugin_BufferSettingsConfig_config(this ILogger logger, object config)
        {
            _SLT00017_Trace_BufferedOutputPlugin_BufferSettingsConfig_config(logger, config, null);
        }
    }
}