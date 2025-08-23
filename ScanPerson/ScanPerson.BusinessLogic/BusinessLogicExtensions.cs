using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Extensions;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Contracts;

namespace ScanPerson.BusinessLogic
{
	/// <summary>
	/// Расширение для добавления сервисов бизнес функциональности.
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
				HtmlWebRuApiKey = GetViriableByName("HTMLWEBRU_API_KEY"),
			};
			services.AddSingleton(secrets);
		}

		private static string GetViriableByName(string variableName)
		{
			return Environment.GetEnvironmentVariable(variableName)
					?? throw new InvalidOperationException(string.Format(Messages.EnvironmentNotFound, variableName));
		}
	}
}
