// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using T2.Cls.LogTransport.Common.Interfaces;
using T2.Cls.LogTransport.Common.Structs;
using T2.CLS.LogTransport.Extensions;
using T2.CLS.LogTransport.Config;

namespace T2.CLS.LoggerExtensions.Clickhouse.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class LogController : ControllerBase
	{
		#region Ctors

		public LogController(IPluginFactory outputFactory, ILogger<LogController> logger, IConfiguration configuration)
		{
			_logger = logger;

            _logger.SLT00003_Debug_LogController_Start();
			_outputFactory = outputFactory;
			_config = new TransportConfiguration();

			configuration?.GetSection("Transport")?.Bind(_config);
            _logger.SLT00002_Debug_LogController_End();
		}

		#endregion

		#region Fields

		private readonly ILogger<LogController> _logger;
		private readonly IPluginFactory _outputFactory;
		private readonly TransportConfiguration _config;
		#endregion

		#region Methods

		[HttpGet]
		public Task<MetricsData> Metrics()
		{
			return Task.Run(() =>
			{
				long pending = 0;
				double rate = 0;
				var pluginCount = 0;

				foreach (var outputPlugin in _outputFactory.OutputPlugins)
				{
					pluginCount++;
					pending += outputPlugin.PendingCount;
					rate += outputPlugin.SendRatePerSecond;
				}

				if (pluginCount > 0)
					rate /= pluginCount;

				var metricsData = new MetricsData(pending, rate);

                _logger.SLT00004_Trace_Method_Metrics_metricData(metricsData);

				return metricsData;
			});
		}

		[HttpPost("{system}")]
		public async Task<IActionResult> Save(string system)
		{
			try
			{
				_logger.SLT00005_Debug_Save_log_from_system_system(system);

				var logTransport = _outputFactory.GetOutput(system);

				if (logTransport == null)
					throw new InvalidOperationException("Log transport is not configured. System:" + system);

				long result = 0;

				using (var streamReader = new StreamReader(Request.Body))
				{
					var content = streamReader.ReadToEndAsync().Result;
					if (_config.ValidateInputData)
					{
						using var stringReader = new StringReader(content);

						var line = string.Empty;
						var hasError = false;

						while ((line = stringReader.ReadLine()) != null)
						{
							try
							{
								var obj = JObject.Parse(line);
							}
							catch (Exception ex)
							{
                                _logger.SLT00007_Error_Invalid_input_line_line(line, ex);
								hasError = true;
							}
						}

						if (hasError)
							return new StatusCodeResult(500);
					}

					result = logTransport.WriteLog(content);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.SLT00006_Error_Not_save_log_for_system_CLS(system, ex);
				throw ex;
			}
		}

		#endregion
	}
	
}