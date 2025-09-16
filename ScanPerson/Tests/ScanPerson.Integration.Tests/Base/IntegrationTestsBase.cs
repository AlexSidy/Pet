using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ScanPerson.Integration.Tests.Base
{
	public abstract class IntegrationTestsBase
	{
		protected const string PersonInfoControllerName = "PersonInfo";

		protected const string JwtSecretKey = "value-does-not-matterButIneedtowriteastringlargerthan256bits";

		public required TestContext TestContext { get; set; }

		protected WebApplicationFactory<Program>? Factory { get; set; }

		protected HttpClient? HttpClient { get; set; }

		protected static void SetTestEnvironment()
		{
			Environment.SetEnvironmentVariable("HTMLWEBRU_API_KEY", "value-does-not-matter");
			Environment.SetEnvironmentVariable("HTMLWEBRU_API_KEY", "value-does-not-matter");
			Environment.SetEnvironmentVariable("JWT_OPTIONS_SECRET_KEY", JwtSecretKey);
			Environment.SetEnvironmentVariable("IS_CACHE_ENABLE", "false");
			Environment.SetEnvironmentVariable("CACHE_EXPIRATION", "2");
		}

		protected static void RemoveFromServices(IServiceCollection services, IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				var personServiceDescriptor = services.SingleOrDefault(
											d => d.ServiceType == type);
				if (personServiceDescriptor != null)
				{
					services.Remove(personServiceDescriptor);
				}
			}
		}
	}
}
