using FluentValidation;

using LD.Sber.GigaChatSDK;
using LD.Sber.GigaChatSDK.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ScanPerson.BusinessLogic.Managers;
using ScanPerson.BusinessLogic.Services;
using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.BusinessLogic.Services.Orchestration;
using ScanPerson.BusinessLogic.Validators;
using ScanPerson.Common.Extensions;
using ScanPerson.Common.Helpers;
using ScanPerson.Models.Options;

using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace ScanPerson.BusinessLogic
{
	/// <summary>
	/// Extension for adding business functionality services..
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
			var serviceOptions = configuration
				.GetSection(ServicesOptions.AppSettingsSection)
				.Get<ServicesOptions>()
				?? new ServicesOptions();
			services.AddSingleton(serviceOptions);
			services.AddSingleton<HttpContextManager>();
			services.AddSecrets();
			services.AddAllImplementations<IPersonInfoService>();
			services.AddSingleton<IPersonInfoServicesAggregator, PersonInfoServicesAggregator>();
			services.AddSingleton(EnviromentHelper.GetFilledFromEnvironment<CacheOptions>());
			services.AddFluentValidationAutoValidation();
			services.AddValidatorsFromAssemblyContaining<PersonInfoRequestValidator>();
			var whiteHostOptions = new HostWhiteListOptions(EnviromentHelper.GetVariableArrayByName("UNAUTHORIZED_TRUSTED_HOSTS"));
			services.AddSingleton(whiteHostOptions);
			services.AddGigaChat();
		}

		/// <summary>
		/// Adds secrets.
		/// </summary>
		/// <param name="services">The services.</param>
		private static void AddSecrets(this IServiceCollection services)
		{
			services.AddSingleton(EnviromentHelper.GetFilledFromEnvironment<ScanPersonSecrets>());
		}

		/// <summary>
		/// Adds secrets.
		/// </summary>
		/// <param name="services">The services.</param>
		private static void AddGigaChat(this IServiceCollection services)
		{
			var key = EnviromentHelper.GetVariableByName("GIGA_CHAT_API_KEY");
			var httpService = new HttpService(true);
			var tokenService = new TokenService(httpService, key, false);
			var gigaChat = new GigaChat(tokenService, httpService, false);
			services.AddSingleton<IGigaChat>(gigaChat);
			services.AddSingleton<IChatAIService, GigaChatService>();
		}
	}
}
