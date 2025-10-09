using Microsoft.AspNetCore.Hosting;

using ScanPerson.Common.Helpers;

using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

using TelegramBots.BusinessLogic;

var environmentName = string.Empty;
var builder = Host.CreateDefaultBuilder(args)
	.ConfigureAppConfiguration((hostingContext, config) =>
	{
		environmentName = hostingContext.HostingEnvironment.EnvironmentName;
		var configPath = Path.Combine(AppContext.BaseDirectory, $"appsettings.{environmentName}.json");
		config
			.AddJsonFile(configPath)
			.AddEnvironmentVariables()
			.Build();
	})
	.ConfigureServices((hostContext, services) =>
	{
		var configuration = hostContext.Configuration;
		var graylogOptions = EnviromentHelper.GetHostOptionsBySectionByName("Graylog", configuration);
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Graylog(new GraylogSinkOptions
			{
				HostnameOrAddress = graylogOptions.Host ?? "graylog",
				Port = graylogOptions.Port ?? 12201,
				Facility = ProjectName,
				MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information,
				TransportType = TransportType.Tcp
			})
			.CreateLogger();
		services.AddBusinessLogicServices(configuration!);
		services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
	});

var host = builder.Build();
if (environmentName != "Staging")
{
	await host.RunAsync();
}

#pragma warning disable S1118
public partial class Program
{
	public const string ProjectName = "TelegramBots.BusinessLogic";
}
#pragma warning restore S1118
