using System.Reflection;

using FluentMigrator.Runner;

using Microsoft.EntityFrameworkCore;

using ScanPerson.Auth.Api.Initializers.Interfaces;
using ScanPerson.Auth.Api.Migrations._2024_12;
using ScanPerson.Auth.Api.Services;
using ScanPerson.Auth.Api.Services.Interfaces;

namespace ScanPerson.Auth.Api
{
	public static class AuthExtensions
	{
		public static void AddScanPersonAuth(this IServiceCollection services, string connectionString)
		{
			services.AddAuthDbContexts(connectionString);
			services.AddMigrations(connectionString);
			services.UpdateDatabase();
			services.AddIdentity();
			services.AddInitializers();
			services.InitData();
			services.AddScoped<ITokenProvider, JwtProvider>();
			services.AddScoped<IUserService, UserService>();
		}

		private static void AddAuthDbContexts(this IServiceCollection services, string connectionString)
		{
			services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(connectionString));
		}

		private static void AddIdentity(this IServiceCollection services)
		{
			services.AddDefaultIdentity<User>(options =>
				{
					options.SignIn.RequireConfirmedAccount = false;
					options.Lockout.MaxFailedAccessAttempts = 5;
					options.SignIn.RequireConfirmedEmail = false;
				})
				.AddRoles<Role>()
				.AddEntityFrameworkStores<AuthDbContext>();
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
					.ScanIn(typeof(InitiaIdentity).Assembly).For.Migrations()
				)

				// Enable logging to console in the FluentMigrator way
				.AddLogging(lb => lb.AddFluentMigratorConsole())
				.UpdateDatabase();
		}

		private static void UpdateDatabase(this IServiceCollection services)
		{
			// Get the service provider
			var serviceProvider = services.BuildServiceProvider(false); ;

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
				initializer.SeedAsync();
			}
		}

		private static void AddAllImplementations<T>(this IServiceCollection services)
		{
			// Находим все типы, которые реализуют интерфейс T
			var implementations = Assembly.GetExecutingAssembly().GetTypes()
				.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

			foreach (var implementation in implementations)
			{
				// Регистрируем каждую реализацию
				services.AddTransient(typeof(T), implementation);
			}
		}
	}
}
