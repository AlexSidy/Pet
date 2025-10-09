using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ScanPerson.Common.Helpers;

using Telegram.Bot;

using TelegramBots.BusinessLogic.Options;
using TelegramBots.BusinessLogic.Services;
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

			services.AddSingleton(serviceOptions);
			services.AddSingleton<ScanPersonBotService>();
			services.AddHostedService<ScanPersonBotPollingWorker>();

			services.AddHttpClient<ScanPersonWebApiService>()
				.ConfigurePrimaryHttpMessageHandler(() =>
			{
				// TODO: remove in task #22.
				var handler = new HttpClientHandler
				{
					ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
				};
				return handler;
			});
		}
	}
}
