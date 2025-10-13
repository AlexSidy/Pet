using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ScanPerson.Common.GrpcServices;
using ScanPerson.Common.Helpers;
using ScanPerson.Common.MapperProfiles;

using Telegram.Bot;

using TelegramBots.BusinessLogic.Options;
using TelegramBots.BusinessLogic.Services;
using TelegramBots.BusinessLogic.Services.Interfaces;
using TelegramBots.BusinessLogic.Workers;


namespace TelegramBots.BusinessLogic
{
	/// <summary>
	/// Extension for adding business functionality services.
	/// </summary>
	public static class BusinessLogicExtensions
	{
		/// <summary>
		/// Adds business logic services.
		/// </summary>
		/// <param name="services">The services.</param>
		/// <param name="configuration">The configuration.</param>
		public static void AddBusinessLogicServices(this IServiceCollection services, IConfiguration configuration)
		{
			var scanPersonBotToken = EnviromentHelper.GetVariableByName("SCAN_PERSON_BOT_TOKEN");
			services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(scanPersonBotToken));

			var serviceOptions = configuration.GetSection(ServicesOptions.AppSettingsSection).Get<ServicesOptions>() ?? new ServicesOptions();
			var grpcOptions = serviceOptions.ScanPersonWebApiServiceOptions.ServiceHostOptions;
			services.AddGrpcClient<GrpcPersonInfo.GrpcPersonInfoClient>(options =>
			{
				options.Address = new Uri($"{grpcOptions!.Host}:{grpcOptions!.Port}");
			});

			services.AddSingleton<IBotService, ScanPersonBotService>();
			services.AddHostedService<ScanPersonBotPollingWorker>();
			services.AddTransient<IApiService, ScanPersonWebApiService>();
			services.AddAutoMapper(cfg =>
			{
				cfg.LicenseKey = EnviromentHelper.GetVariableByName("AUTO_MAPPER_LICENSE_KEY");
				cfg.AddProfile<GrpcProfile>();
			});

		}
	}
}
