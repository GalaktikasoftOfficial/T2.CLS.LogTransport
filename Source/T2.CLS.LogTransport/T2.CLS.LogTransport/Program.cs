// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace T2.CLS.LoggerExtensions.Clickhouse
{
	public class Program
	{
		#region Methods

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			var host = WebHost.CreateDefaultBuilder(args);

			return Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration(builder =>
				{
					builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
					builder.AddJsonFile("appsettings.logging.json", optional: true, reloadOnChange: true);

					var environment = host.GetSetting("environment");
					var readEnvironment = string.Empty;

					foreach (var envPart in environment.Split('.'))
					{
						readEnvironment += '.' + envPart;

						builder.AddJsonFile($"appsettings{readEnvironment}.json", optional: true, reloadOnChange: true)
							.AddJsonFile($"appsettings.logging{readEnvironment}.json", optional: true, reloadOnChange: true);
					}

					if (args.Contains("--forwarder"))
						builder.AddJsonFile("appsettings.forwarder.json");

					if (args.Contains("--worker"))
						builder.AddJsonFile("appsettings.worker.json");

				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
					webBuilder.UseNLog();
				});
		}

		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		#endregion
	}
}