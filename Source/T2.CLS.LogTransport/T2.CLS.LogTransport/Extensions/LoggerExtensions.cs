using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using T2.CLS.LogTransport.Properties;

namespace T2.CLS.LogTransport.Extensions
{
    public static class LoggerExtensions
    {
		private static readonly Action<ILogger, object, Exception> _CLT00001_Warning_Property_propertyName_ignored;
		private static readonly Action<ILogger, object, Exception> _CLT00002_Error_Error_sending_log_in_ClickHouse_content;
		private static readonly Action<ILogger, object, object, Exception> _CLT00003_Information_Created_Output_plugin_Type_pluginName_SystemName_systemName;
		private static readonly Action<ILogger, Exception> _SLT00001_Trace_LogController_created;
		private static readonly Action<ILogger, Exception> _SLT00002_Debug_LogController_End;
		private static readonly Action<ILogger, Exception> _SLT00003_Debug_LogController_Start;
		private static readonly Action<ILogger, object, Exception> _SLT00004_Trace_Method_Metrics_metricData;
		private static readonly Action<ILogger, object, Exception> _SLT00005_Debug_Save_log_from_system_system;
		private static readonly Action<ILogger, object, Exception> _SLT00006_Error_Not_save_log_for_system_CLS;
		private static readonly Action<ILogger, object, Exception> _SLT00007_Error_Invalid_input_line_line;
		private static readonly Action<ILogger, Exception> _SLT00008_Debug_LogMetrics_Start;
		private static readonly Action<ILogger, Exception> _SLT00009_Debug_LogMetrics_End;
		private static readonly Action<ILogger, Exception> _SLT00010_Trace_PluginFactory_Start;
		private static readonly Action<ILogger, Exception> _SLT00011_Debug_PluginFactory_End;
		private static readonly Action<ILogger, Exception> _SLT00012_Debug_PluginFactory;
		private static readonly Action<ILogger, object, object, Exception> _SLT00013_Debug_Create_Output_pluginType_plugin_SystemName_systemName;
		private static readonly Action<ILogger, Exception> _SLT00014_Error_Plugin_not_created;
		private static readonly Action<ILogger, object, Exception> _SLT00019_Trace_Main_buffer_settings_config_bufferSettingsConfig;
		private static readonly Action<ILogger, Exception> _SLT00020_Debug_Child_settings_empty_Parent_config_is_applied;
		private static readonly Action<ILogger, Exception> _SLT00021_Debug_Child_and_parent_settings_merged;
		private static readonly Action<ILogger, object, Exception> _SLT00022_Trace_Plugin_output_config_clickHousePluginConfig;
		private static readonly Action<ILogger, object, Exception> _SLT00023_Trace_Plugin_output_config_forwardOutputConfig;
		private static readonly Action<ILogger, Exception> _SLT00024_Debug_Child_settings_empty_Parent_config_is_applied;
		private static readonly Action<ILogger, Exception> _SLT00025_Debug_Child_and_parent_settings_merged;

        static LoggerExtensions()
        {
            _CLT00001_Warning_Property_propertyName_ignored = LoggerMessage.Define<object>(
                LogLevel.Warning,
                new EventId(1, nameof(Logs.CLT00001)),
                Logs.CLT00001);
            _CLT00002_Error_Error_sending_log_in_ClickHouse_content = LoggerMessage.Define<object>(
                LogLevel.Error,
                new EventId(2, nameof(Logs.CLT00002)),
                Logs.CLT00002);
            _CLT00003_Information_Created_Output_plugin_Type_pluginName_SystemName_systemName = LoggerMessage.Define<object, object>(
                LogLevel.Information,
                new EventId(3, nameof(Logs.CLT00003)),
                Logs.CLT00003);
            _SLT00001_Trace_LogController_created = LoggerMessage.Define(
                LogLevel.Trace,
                new EventId(1, nameof(Logs.SLT00001)),
                Logs.SLT00001);
            _SLT00002_Debug_LogController_End = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(2, nameof(Logs.SLT00002)),
                Logs.SLT00002);
            _SLT00003_Debug_LogController_Start = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(3, nameof(Logs.SLT00003)),
                Logs.SLT00003);
            _SLT00004_Trace_Method_Metrics_metricData = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(4, nameof(Logs.SLT00004)),
                Logs.SLT00004);
            _SLT00005_Debug_Save_log_from_system_system = LoggerMessage.Define<object>(
                LogLevel.Debug,
                new EventId(5, nameof(Logs.SLT00005)),
                Logs.SLT00005);
            _SLT00006_Error_Not_save_log_for_system_CLS = LoggerMessage.Define<object>(
                LogLevel.Error,
                new EventId(6, nameof(Logs.SLT00006)),
                Logs.SLT00006);
            _SLT00007_Error_Invalid_input_line_line = LoggerMessage.Define<object>(
                LogLevel.Error,
                new EventId(7, nameof(Logs.SLT00007)),
                Logs.SLT00007);
            _SLT00008_Debug_LogMetrics_Start = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(8, nameof(Logs.SLT00008)),
                Logs.SLT00008);
            _SLT00009_Debug_LogMetrics_End = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(9, nameof(Logs.SLT00009)),
                Logs.SLT00009);
            _SLT00010_Trace_PluginFactory_Start = LoggerMessage.Define(
                LogLevel.Trace,
                new EventId(10, nameof(Logs.SLT00010)),
                Logs.SLT00010);
            _SLT00011_Debug_PluginFactory_End = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(11, nameof(Logs.SLT00011)),
                Logs.SLT00011);
            _SLT00012_Debug_PluginFactory = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(12, nameof(Logs.SLT00012)),
                Logs.SLT00012);
            _SLT00013_Debug_Create_Output_pluginType_plugin_SystemName_systemName = LoggerMessage.Define<object, object>(
                LogLevel.Debug,
                new EventId(13, nameof(Logs.SLT00013)),
                Logs.SLT00013);
            _SLT00014_Error_Plugin_not_created = LoggerMessage.Define(
                LogLevel.Error,
                new EventId(14, nameof(Logs.SLT00014)),
                Logs.SLT00014);
            _SLT00019_Trace_Main_buffer_settings_config_bufferSettingsConfig = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(19, nameof(Logs.SLT00019)),
                Logs.SLT00019);
            _SLT00020_Debug_Child_settings_empty_Parent_config_is_applied = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(20, nameof(Logs.SLT00020)),
                Logs.SLT00020);
            _SLT00021_Debug_Child_and_parent_settings_merged = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(21, nameof(Logs.SLT00021)),
                Logs.SLT00021);
            _SLT00022_Trace_Plugin_output_config_clickHousePluginConfig = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(22, nameof(Logs.SLT00022)),
                Logs.SLT00022);
            _SLT00023_Trace_Plugin_output_config_forwardOutputConfig = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(23, nameof(Logs.SLT00023)),
                Logs.SLT00023);
            _SLT00024_Debug_Child_settings_empty_Parent_config_is_applied = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(24, nameof(Logs.SLT00024)),
                Logs.SLT00024);
            _SLT00025_Debug_Child_and_parent_settings_merged = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(25, nameof(Logs.SLT00025)),
                Logs.SLT00025);
        }
        
        /// <summary>
        /// Property {propertyName} ignored
        /// </summary>
        public static void CLT00001_Warning_Property_propertyName_ignored(this ILogger logger, object propertyName)
        {
            _CLT00001_Warning_Property_propertyName_ignored(logger, propertyName, null);
        }

        /// <summary>
        /// Error sending log in ClickHouse: {content}
        /// </summary>
        public static void CLT00002_Error_Error_sending_log_in_ClickHouse_content(this ILogger logger, object content)
        {
            _CLT00002_Error_Error_sending_log_in_ClickHouse_content(logger, content, null);
        }

        /// <summary>
        /// Created Output plugin. Type '{pluginName}'. SystemName '{systemName}'
        /// </summary>
        public static void CLT00003_Information_Created_Output_plugin_Type_pluginName_SystemName_systemName(this ILogger logger, object pluginName, object systemName)
        {
            _CLT00003_Information_Created_Output_plugin_Type_pluginName_SystemName_systemName(logger, pluginName, systemName, null);
        }

        /// <summary>
        /// LogController created
        /// </summary>
        public static void SLT00001_Trace_LogController_created(this ILogger logger)
        {
            _SLT00001_Trace_LogController_created(logger, null);
        }

        /// <summary>
        /// ~LogController End
        /// </summary>
        public static void SLT00002_Debug_LogController_End(this ILogger logger)
        {
            _SLT00002_Debug_LogController_End(logger, null);
        }

        /// <summary>
        /// ~LogController Start
        /// </summary>
        public static void SLT00003_Debug_LogController_Start(this ILogger logger)
        {
            _SLT00003_Debug_LogController_Start(logger, null);
        }

        /// <summary>
        /// Method Metrics() =>{metricData}
        /// </summary>
        public static void SLT00004_Trace_Method_Metrics_metricData(this ILogger logger, object metricData)
        {
            _SLT00004_Trace_Method_Metrics_metricData(logger, metricData, null);
        }

        /// <summary>
        /// Save log from system: {system}
        /// </summary>
        public static void SLT00005_Debug_Save_log_from_system_system(this ILogger logger, object system)
        {
            _SLT00005_Debug_Save_log_from_system_system(logger, system, null);
        }

        /// <summary>
        /// Not save log for system '{CLS}'
        /// </summary>
        public static void SLT00006_Error_Not_save_log_for_system_CLS(this ILogger logger, object CLS, Exception ex)
        {
            _SLT00006_Error_Not_save_log_for_system_CLS(logger, CLS, ex);
        }

        /// <summary>
        /// Invalid input line: {line}
        /// </summary>
        public static void SLT00007_Error_Invalid_input_line_line(this ILogger logger, object line, Exception ex)
        {
            _SLT00007_Error_Invalid_input_line_line(logger, line, ex);
        }

        /// <summary>
        /// ~LogMetrics Start
        /// </summary>
        public static void SLT00008_Debug_LogMetrics_Start(this ILogger logger)
        {
            _SLT00008_Debug_LogMetrics_Start(logger, null);
        }

        /// <summary>
        /// ~LogMetrics End
        /// </summary>
        public static void SLT00009_Debug_LogMetrics_End(this ILogger logger)
        {
            _SLT00009_Debug_LogMetrics_End(logger, null);
        }

        /// <summary>
        /// ~PluginFactory Start
        /// </summary>
        public static void SLT00010_Trace_PluginFactory_Start(this ILogger logger)
        {
            _SLT00010_Trace_PluginFactory_Start(logger, null);
        }

        /// <summary>
        /// ~PluginFactory End
        /// </summary>
        public static void SLT00011_Debug_PluginFactory_End(this ILogger logger)
        {
            _SLT00011_Debug_PluginFactory_End(logger, null);
        }

        /// <summary>
        /// ~PluginFactory
        /// </summary>
        public static void SLT00012_Debug_PluginFactory(this ILogger logger)
        {
            _SLT00012_Debug_PluginFactory(logger, null);
        }

        /// <summary>
        /// Create Output {pluginType} plugin. SystemName: {systemName}
        /// </summary>
        public static void SLT00013_Debug_Create_Output_pluginType_plugin_SystemName_systemName(this ILogger logger, object pluginType, object systemName)
        {
            _SLT00013_Debug_Create_Output_pluginType_plugin_SystemName_systemName(logger, pluginType, systemName, null);
        }

        /// <summary>
        /// Plugin not created.
        /// </summary>
        public static void SLT00014_Error_Plugin_not_created(this ILogger logger, Exception ex)
        {
            _SLT00014_Error_Plugin_not_created(logger, ex);
        }

        /// <summary>
        /// Main buffer settings config: {bufferSettingsConfig}
        /// </summary>
        public static void SLT00019_Trace_Main_buffer_settings_config_bufferSettingsConfig(this ILogger logger, object bufferSettingsConfig)
        {
            _SLT00019_Trace_Main_buffer_settings_config_bufferSettingsConfig(logger, bufferSettingsConfig, null);
        }

        /// <summary>
        /// Child settings empty. Parent config is applied.
        /// </summary>
        public static void SLT00020_Debug_Child_settings_empty_Parent_config_is_applied(this ILogger logger)
        {
            _SLT00020_Debug_Child_settings_empty_Parent_config_is_applied(logger, null);
        }

        /// <summary>
        /// Child and parent settings merged.
        /// </summary>
        public static void SLT00021_Debug_Child_and_parent_settings_merged(this ILogger logger)
        {
            _SLT00021_Debug_Child_and_parent_settings_merged(logger, null);
        }

        /// <summary>
        /// Plugin output config: {clickHousePluginConfig}
        /// </summary>
        public static void SLT00022_Trace_Plugin_output_config_clickHousePluginConfig(this ILogger logger, object clickHousePluginConfig)
        {
            _SLT00022_Trace_Plugin_output_config_clickHousePluginConfig(logger, clickHousePluginConfig, null);
        }

        /// <summary>
        /// Plugin output config: {forwardOutputConfig}
        /// </summary>
        public static void SLT00023_Trace_Plugin_output_config_forwardOutputConfig(this ILogger logger, object forwardOutputConfig)
        {
            _SLT00023_Trace_Plugin_output_config_forwardOutputConfig(logger, forwardOutputConfig, null);
        }

        /// <summary>
        /// Child settings empty. Parent config is applied.
        /// </summary>
        public static void SLT00024_Debug_Child_settings_empty_Parent_config_is_applied(this ILogger logger)
        {
            _SLT00024_Debug_Child_settings_empty_Parent_config_is_applied(logger, null);
        }

        /// <summary>
        /// Child and parent settings merged.
        /// </summary>
        public static void SLT00025_Debug_Child_and_parent_settings_merged(this ILogger logger)
        {
            _SLT00025_Debug_Child_and_parent_settings_merged(logger, null);
        }
    }
}