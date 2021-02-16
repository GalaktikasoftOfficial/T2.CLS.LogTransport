using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using T2.Cls.LogTransport.OutputPlugin.ClickHouse.Properties;

namespace T2.Cls.LogTransport.OutputPlugin.ClickHouse.Extensions
{
    public static class LoggerExtensions
    {
		private static readonly Action<ILogger, object, Exception> _SLT00018_Debug_ClickhouseOutputPlugin_outputConfig_outputConfig;

        static LoggerExtensions()
        {
            _SLT00018_Debug_ClickhouseOutputPlugin_outputConfig_outputConfig = LoggerMessage.Define<object>(
                LogLevel.Debug,
                new EventId(18, nameof(Logs.SLT00018)),
                Logs.SLT00018);
        }
        
        /// <summary>
        /// ~ClickhouseOutputPlugin. outputConfig: {outputConfig}
        /// </summary>
        public static void SLT00018_Debug_ClickhouseOutputPlugin_outputConfig_outputConfig(this ILogger logger, object outputConfig)
        {
            _SLT00018_Debug_ClickhouseOutputPlugin_outputConfig_outputConfig(logger, outputConfig, null);
        }
    }
}