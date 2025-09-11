using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Extensions;
using ScanPerson.Common.Helpers;
using ScanPerson.Models.Options;

namespace ScanPerson.BusinessLogic
{
	/// <summary>
	/// Extension for adding business functionality services..
	/// </summary>
	public static class BusinessLogicExtensions
	{
		public static void AddBusinessLogicServices(this IServiceCollection services, IConfiguration configuration)
		{
			var serviceOptions = configuration.GetSection(ServicesOptions.AppSettingsSection).Get<ServicesOptions>() ?? new ServicesOptions();
			services.AddSingleton(serviceOptions);
			services.AddSecrets();
			services.AddAllImplementations<IPersonInfoService>();
			services.AddSingleton<IPersonInfoServicesAggregator, PersonInfoServicesAggregator>();
		}

		private static void AddSecrets(this IServiceCollection services)
		{
			var secrets = new ScanPersonSecrets
			{
				HtmlWebRuApiKey = EnviromentHelper.GetViriableByName("HTMLWEBRU_API_KEY"),
			};
			services.AddSingleton(secrets);
		}
	}
}
