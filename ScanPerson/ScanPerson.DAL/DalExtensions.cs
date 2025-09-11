using FluentMigrator.Runner;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ScanPerson.Common.Extensions;
using ScanPerson.DAL.Contexts;
using ScanPerson.DAL.Initializers.Interfaces;
using ScanPerson.DAL.Migrations._2024_12;

namespace ScanPerson.DAL
{
	public static class DalExtensions
	{
		public static void AddDalServices(this IServiceCollection services, string connectionString)
		{
			services.AddScanPersonDbContexts(connectionString);

			// override default VersionTable 
			services.AddMigrations(connectionString);
			services.UpdateDatabase();
			services.AddInitializers();
			services.InitData();
		}

		private static void AddScanPersonDbContexts(this IServiceCollection services, string connectionString)
		{
			services.AddDbContext<ScanPersonDbContext>(options => options.UseNpgsql(connectionString));
		}

		public static void AddMigrations(this IServiceCollection services, string connectionString)
		{
			services
				// Add common FluentMigrator services
				.AddFluentMigratorCore()
				.ConfigureRunner(runnerBuilder => runnerBuilder
					// Add Postgres support to FluentMigrator
					.AddPostgres()
					// Set the connection string
					.WithGlobalConnectionString(connectionString)
					// Define the assembly containing the migrations
					.ScanIn(typeof(InitialSheme).Assembly).For.Migrations()
				)

				// Enable logging to console in the FluentMigrator way
				.AddLogging(lb => lb.AddFluentMigratorConsole())
				.UpdateDatabase();
		}

		private static void UpdateDatabase(this IServiceCollection services)
		{
			// Get the service provider
			var serviceProvider = services.BuildServiceProvider(false);

			// Instantiate the runner
			var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

			// Execute the migrations
			runner.MigrateUp();
		}

		private static void AddInitializers(this IServiceCollection services)
		{
			services.AddAllImplementations<IInitializer>();
		}

		private static void InitData(this IServiceCollection services)
		{
			var serviceProvider = services.BuildServiceProvider(false);

			var initializers = serviceProvider.GetServices<IInitializer>();
			foreach (var initializer in initializers)
			{
				initializer.Seed();
			}
		}
	}
}
