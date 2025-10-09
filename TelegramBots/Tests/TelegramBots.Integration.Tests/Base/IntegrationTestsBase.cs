using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using Telegram.Bot;

using TelegramBots.BusinessLogic.Services;
using TelegramBots.BusinessLogic.Workers;

public class IntegrationTestsBase : IDisposable
{
	public required TestContext TestContext { get; set; }

	protected readonly WebApplicationFactory<Program> Factory;
	protected readonly Mock<ITelegramBotClient> MockBotClient;
	protected readonly Mock<ILogger<ScanPersonWebApiService>> MockWebApiLogger;
	protected readonly Mock<ILogger<ScanPersonBotPollingWorker>> MockWorkerLogger;
	protected readonly Mock<HttpMessageHandler> MockHttpMessageHandler;

	public IntegrationTestsBase()
	{
		MockBotClient = new Mock<ITelegramBotClient>();
		MockHttpMessageHandler = new Mock<HttpMessageHandler>();
		MockWebApiLogger = new Mock<ILogger<ScanPersonWebApiService>>();
		MockWorkerLogger = new Mock<ILogger<ScanPersonBotPollingWorker>>();
		Factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder =>
			{
				builder.UseEnvironment("Staging");
				SetTestEnvironment();
				builder.ConfigureServices(services =>
				{
					var servicesToRemove = new List<Type>
					{
						typeof(IHostedService),
						typeof(ITelegramBotClient),
						typeof(IConfigureOptions<LoggerFilterOptions>)
					};
					var implementationToRemove = new List<Type>
					{
						typeof(ScanPersonBotPollingWorker)
					};

					var descriptors = services.Where(d =>
						servicesToRemove.Contains(d.ServiceType) ||
						implementationToRemove.Contains(d.ImplementationType ?? d.ServiceType))
					.ToList();
					foreach (var descriptor in descriptors)
					{
						services.Remove(descriptor);
					}

					services.AddSingleton(MockBotClient.Object);
					services.AddSingleton(MockWebApiLogger.Object);
					services.AddTransient<ScanPersonWebApiService>();
					services.AddHttpClient<ScanPersonWebApiService>()
						.ConfigurePrimaryHttpMessageHandler(() => MockHttpMessageHandler.Object);
				});
			});
	}

	protected static void SetTestEnvironment()
	{
		Environment.SetEnvironmentVariable("SCAN_PERSON_BOT_TOKEN", "7123456789:AAAAABB0C1DDD22asdfghYFkpChjklg-0");
	}

	public void Dispose()
	{
		Factory.Dispose();
	}
}