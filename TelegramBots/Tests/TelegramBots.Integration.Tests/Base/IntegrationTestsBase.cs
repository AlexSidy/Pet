using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using ScanPerson.Common.GrpcServices;

using Telegram.Bot;

using TelegramBots.BusinessLogic.Services;
using TelegramBots.BusinessLogic.Workers;

namespace TelegramBots.Integration.Tests.Base;

public class IntegrationTestsBase
{
	public required TestContext TestContext { get; set; }

	protected readonly WebApplicationFactory<Program> Factory;
	protected readonly Mock<ITelegramBotClient> MockBotClient;
	protected readonly Mock<ILogger<ScanPersonWebApiService>> MockWebApiLogger;
	protected readonly Mock<ILogger<ScanPersonBotPollingWorker>> MockWorkerLogger;
	protected readonly Mock<GrpcPersonInfo.GrpcPersonInfoClient> MockGrpcClient;

	public IntegrationTestsBase()
	{
		MockBotClient = new Mock<ITelegramBotClient>();
		MockWebApiLogger = new Mock<ILogger<ScanPersonWebApiService>>();
		MockWorkerLogger = new Mock<ILogger<ScanPersonBotPollingWorker>>();
		MockGrpcClient = new Mock<GrpcPersonInfo.GrpcPersonInfoClient>();

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
						typeof(IConfigureOptions<LoggerFilterOptions>),
						typeof(GrpcPersonInfo.GrpcPersonInfoClient)
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
					services.AddSingleton(MockGrpcClient.Object);
				});
			});
	}

	protected static void SetTestEnvironment()
	{
		Environment.SetEnvironmentVariable("SCAN_PERSON_BOT_TOKEN", "7123456789:AAAAABB0C1DDD22asdfghYFkpChjklg-0");
		Environment.SetEnvironmentVariable("AUTO_MAPPER_LICENSE_KEY", "mapper:AAAAABB0C1DDD22asdfghYFkpChjklg-0");
	}
}