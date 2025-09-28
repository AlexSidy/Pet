using FluentValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.BusinessLogic.Validators;
using ScanPerson.Common.Extensions;
using ScanPerson.Common.Helpers;
using ScanPerson.Models.Options;

using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors;

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
			var serviceOptions = configuration.GetSection(ServicesOptions.AppSettingsSection).Get<ServicesOptions>() ?? new ServicesOptions();
			services.AddSingleton(serviceOptions);
			services.AddSecrets();
			services.AddAllImplementations<IPersonInfoService>();
			services.AddSingleton<IPersonInfoServicesAggregator, PersonInfoServicesAggregator>();
			services.AddSingleton(EnviromentHelper.GetFilledFromEnvironment<CacheOptions>());
			services.AddFluentValidationAutoValidation();
			services.AddValidatorsFromAssemblyContaining<PersonInfoRequestValidator>();
		}

		/// <summary>
		/// Adds secrets.
		/// </summary>
		/// <param name="services">The services.</param>
		private static void AddSecrets(this IServiceCollection services)
		{
			services.AddSingleton(EnviromentHelper.GetFilledFromEnvironment<ScanPersonSecrets>());
		}
	}
}
