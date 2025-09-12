using FluentMigrator.Runner;

using Identity.Api.Initializers.Interfaces;
using Identity.Api.Migrations._2024_12;
using Identity.Api.Services;
using Identity.Api.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

using ScanPerson.Common.Extensions;

namespace Identity.Api
{
	/// <summary>
	/// Extension methods for configuring authentication services within the Identity API.
	/// </summary>
	public static class AuthExtensions
	{
		/// <summary>
		/// Adds authentication-related services to the DI container, including database context, migrations, identity management, initializers, and core authentication services.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		/// <param name="connectionString">The connection string for the authentication database.</param>
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

		/// <summary>
		/// Configures and adds the authentication database context (<see cref="AuthDbContext"/>) to the DI container.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		/// <param name="connectionString">The connection string for the PostgreSQL database.</param>
		private static void AddAuthDbContexts(this IServiceCollection services, string connectionString)
		{
			services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(connectionString));
		}

		/// <summary>
		/// Configures ASP.NET Core Identity for the application.
		/// Sets up user, role, and sign-in options.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		private static void AddIdentity(this IServiceCollection services)
		{
			services.AddDefaultIdentity<User>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false; // Disables email confirmation requirement for sign-in.
				options.Lockout.MaxFailedAccessAttempts = 5; // Sets the maximum number of failed access attempts before lockout.
				options.SignIn.RequireConfirmedEmail = false; // Disables email confirmation requirement for account creation.
			})
				.AddRoles<Role>() // Enables role management.
				.AddEntityFrameworkStores<AuthDbContext>(); // Specifies the DbContext for Identity data storage.
		}

		/// <summary>
		/// Configures FluentMigrator for database schema management.
		/// Sets up PostgreSQL support, connection string, and scans for migrations.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		/// <param name="connectionString">The connection string for the PostgreSQL database.</param>
		public static void AddMigrations(this IServiceCollection services, string connectionString)
		{
			services
				// Add common FluentMigrator services.
				.AddFluentMigratorCore()
				.ConfigureRunner(runnerBuilder => runnerBuilder
					// Add PostgreSQL support to FluentMigrator.
					.AddPostgres()
					// Set the global connection string for migrations.
					.WithGlobalConnectionString(connectionString)
					// Define the assembly containing the migrations to scan.
					.ScanIn(typeof(InitiaIdentity).Assembly).For.Migrations()
				)
				// Enable logging to the console in the FluentMigrator way.
				.AddLogging(lb => lb.AddFluentMigratorConsole())
				.UpdateDatabase(); // Immediately applies pending migrations.
		}

		/// <summary>
		/// Builds the service provider and executes all pending database migrations up to the latest version.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> containing the configured services.</param>
		private static void UpdateDatabase(this IServiceCollection services)
		{
			// Get the service provider and execute the migrations.
			services.BuildServiceProvider(false) // Build a temporary service provider for migration execution.
				.GetRequiredService<IMigrationRunner>() // Get the migration runner.
				.MigrateUp(); // Apply all pending migrations.
		}

		/// <summary>
		/// Registers all classes that implement the <see cref="IInitializer"/> interface as transient services.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		private static void AddInitializers(this IServiceCollection services)
		{
			services.AddAllImplementations<IInitializer>(); // Extension method to add all implementations of an interface.
		}

		/// <summary>
		/// Initializes the application data by executing all registered initializers.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> containing the configured services.</param>
		private static void InitData(this IServiceCollection services)
		{
			var serviceProvider = services.BuildServiceProvider(false); // Build a temporary service provider.

			var initializers = serviceProvider.GetServices<IInitializer>(); // Get all registered initializers.
			foreach (var initializer in initializers)
			{
				initializer.SeedAsync(); // Execute the seeding logic for each initializer.
			}
		}
	}
}