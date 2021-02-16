// Copyright (C) 2019 Topsoft (https://topsoft.by)

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using T2.Cls.LogTransport.Common.Interfaces;
using T2.CLS.LoggerExtensions.Clickhouse.Services;

namespace T2.CLS.LoggerExtensions.Clickhouse
{
	public class Startup
	{
		#region Ctors

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		#endregion

		#region Properties

		public IConfiguration Configuration { get; }

		#endregion

		#region Methods

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			var nLogConfig = Configuration.GetSection("Logging:NLog");

			if (nLogConfig != null) 
				LogManager.Configuration = new NLogLoggingConfiguration(nLogConfig);

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

			var pf = app.ApplicationServices.GetService<IPluginFactory>();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<ILogMetrics, LogMetrics>();
			services.AddSingleton<IPluginFactory, PluginFactory>();
			services.AddLogging(builder =>
			{
				var nLogConfig = Configuration.GetSection("Logging:NLog");

				if (nLogConfig != null)
					builder.AddNLog(nLogConfig);
			});
			services.AddControllers();
		}

		#endregion
	}
}