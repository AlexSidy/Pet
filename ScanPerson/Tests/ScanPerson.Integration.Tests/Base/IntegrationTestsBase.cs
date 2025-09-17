using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

using Xunit;


namespace ScanPerson.Integration.Tests.Base
{
	public abstract class IntegrationTestsBase : IAsyncLifetime
	{
		protected const string PersonInfoControllerName = "PersonInfo";

		protected const string JwtSecretKey = "value-does-not-matterButIneedtowriteastringlargerthan256bits";

		public required TestContext TestContext { get; set; }

		private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
			.WithDatabase("test_db")
			.WithUsername("testuser")
			.WithPassword("testpassword")
			.Build();

		protected WebApplicationFactory<Program>? Factory { get; set; }

		protected HttpClient? HttpClient { get; set; }

		private static string _connectionString = string.Empty;

		public abstract Task InitializeAsync();

		[TestCleanup]
		public async Task DisposeAsync()
		{
			await _postgreSqlContainer.DisposeAsync();
		}

		protected static void SetTestEnvironment()
		{
			Environment.SetEnvironmentVariable("AUTO_MAPPER_LICENSE_KEY", "value-does-not-matter");
			Environment.SetEnvironmentVariable("HTMLWEBRU_API_KEY", "value-does-not-matter");
			Environment.SetEnvironmentVariable("JWT_OPTIONS_SECRET_KEY", JwtSecretKey);
			Environment.SetEnvironmentVariable("IS_CACHE_ENABLE", "false");
			Environment.SetEnvironmentVariable("CACHE_EXPIRATION", "2");
			Environment.SetEnvironmentVariable("ConnectionStrings__ScanPersonDb", _connectionString!);
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

		protected virtual async Task InitializeBdAndSetConnectionStringAsync()
		{
			await _postgreSqlContainer.StartAsync(TestContext.CancellationTokenSource.Token);
			_connectionString = _postgreSqlContainer.GetConnectionString();
		}
	}
}
