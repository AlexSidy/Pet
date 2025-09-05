using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ScanPerson.Integration.Tests.Base
{
	[TestClass]
	public abstract class IntegrationTestsBase
	{
		protected const string PersonInfoControllerName = "PersonInfo";

		public TestContext TestContext { get; set; }

		protected WebApplicationFactory<Program>? Factory { get; set; }

		protected HttpClient? HttpClient { get; set; }

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
