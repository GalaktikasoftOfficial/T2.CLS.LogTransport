// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using T2.CLS.LoggerExtensions.Core.Config;
using T2.Cls.LogTransport.Common.Interfaces;
using T2.Cls.LogTransport.OutputPlugin.ClickHouse.Config;
using T2.Cls.LogTransport.OutputPlugin.ClickHouse.Output;
using T2.Cls.LogTransport.OutputPlugin.Forward.Config;
using T2.Cls.LogTransport.OutputPlugin.Forward.Output;
using T2.CLS.LogTransport.Extensions;

namespace T2.CLS.LoggerExtensions.Clickhouse.Services
{
	internal sealed class PluginFactory : IPluginFactory
	{
		#region Properties

		private BufferSettingsConfig _bufferSettingsConfig { get; set; }

		#endregion //Properties

		#region Ctors

		public PluginFactory(IConfiguration configuration, ILogMetrics metrics, ILoggerFactory loggerFactory)
		{
			_metrics = metrics;
			_loggerFactory = loggerFactory;

			_logger = _loggerFactory.CreateLogger<PluginFactory>();

			_logger.SLT00012_Debug_PluginFactory();
			try
			{
				var outputPluginsSection = configuration.GetSection("OutputPlugins");

				_bufferSettingsConfig = new BufferSettingsConfig();
				var bufferConfiguration = configuration.GetSection("BufferSettings");
				if (bufferConfiguration != null)
					bufferConfiguration.Bind(_bufferSettingsConfig);

				if (outputPluginsSection == null)
					return;

				foreach (var pluginSection in outputPluginsSection.GetChildren())
				{
					var pluginName = pluginSection["Output"];
					var systemName = pluginSection["System"];

					if (systemName == null || pluginName == null)
						continue;

					_configs[systemName] = CreateOutputPlugin(pluginName, systemName, pluginSection);
				}
			}
			catch (Exception e)
			{
				_logger.SLT00014_Error_Plugin_not_created(e);
				throw e;
			}
			finally
			{
                _logger.SLT00011_Debug_PluginFactory_End();
			}
		}

		#endregion

		#region Methods

		private IOutputPlugin CreateOutputPlugin(string pluginType, string systemName, IConfigurationSection configurationSection)
		{
			_logger.SLT00013_Debug_Create_Output_pluginType_plugin_SystemName_systemName(pluginType, systemName);
            _logger.SLT00019_Trace_Main_buffer_settings_config_bufferSettingsConfig(_bufferSettingsConfig);

			if (pluginType == "Clickhouse")
			{
				var clickHousePluginConfig = new ClickhouseOutputConfig();

				configurationSection.Bind(clickHousePluginConfig);
                _logger.SLT00022_Trace_Plugin_output_config_clickHousePluginConfig(clickHousePluginConfig);

				if (clickHousePluginConfig.BufferSettings == null)
				{
					clickHousePluginConfig.BufferSettings = _bufferSettingsConfig;
                    _logger.SLT00020_Debug_Child_settings_empty_Parent_config_is_applied();
				}
				else
				{
					clickHousePluginConfig.BufferSettings.JoinWithParentBufferConfig(_bufferSettingsConfig);
					_logger.SLT00021_Debug_Child_and_parent_settings_merged();
				}

				return new ClickhouseOutputPlugin(clickHousePluginConfig, _metrics, _loggerFactory);
			}

			if (pluginType == "Forward")
			{
				var forwardOutputConfig = new ForwardOutputConfig();

				configurationSection.Bind(forwardOutputConfig);
                _logger.SLT00023_Trace_Plugin_output_config_forwardOutputConfig(forwardOutputConfig);

				if (forwardOutputConfig.BufferSettings == null)
				{
					forwardOutputConfig.BufferSettings = _bufferSettingsConfig;
                    _logger.SLT00024_Debug_Child_settings_empty_Parent_config_is_applied();
				}
				else
				{
					forwardOutputConfig.BufferSettings.JoinWithParentBufferConfig(_bufferSettingsConfig);
                    _logger.SLT00025_Debug_Child_and_parent_settings_merged();
				}
				
				return new ForwardOutputPlugin(forwardOutputConfig, _loggerFactory);
			}

			return null;
		}

		#endregion

		#region Fields

		private readonly Dictionary<string, IOutputPlugin> _configs = new Dictionary<string, IOutputPlugin>();

		private readonly ILogMetrics _metrics;
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger _logger;

		#endregion

		#region Interface Implementations

		#region IPluginFactory

		public IOutputPlugin GetOutput(string system)
		{
			return _configs.TryGetValue(system, out var buffer) ? buffer : null;
		}

		public IEnumerable<IOutputPlugin> OutputPlugins => _configs.Values;

		#endregion

		#endregion
	}
}