using Identity.Api.Migrations._2024_12;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api
{
	/// <summary>
	/// The database context for the Identity service, extending IdentityDbContext.
	/// This context is used for managing users, roles, and user claims.
	/// </summary>
	public class AuthDbContext : IdentityDbContext<User, Role, long>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthDbContext"/> class.
		/// </summary>
		/// <param name="options">The options to be used by the DbContext.</param>
		public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
		{
		}

		/// <summary>
		/// Configures the schema for the database models.
		/// </summary>
		/// <param name="builder">The builder used to construct the model for this context.</param>
		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.HasDefaultSchema(InitiaIdentity.AuthSchema);
			base.OnModelCreating(builder);
		}
	}

	/// <summary>
	/// Represents a user in the identity system, extending the default IdentityUser.
	/// The primary key is of type long.
	/// </summary>
	public class User : IdentityUser<long>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="User"/> class.
		/// </summary>
		public User() : base() { }
	}

	/// <summary>
	/// Represents a role in the identity system, extending the default IdentityRole.
	/// The primary key is of type long.
	/// </summary>
	public class Role : IdentityRole<long>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Role"/> class.
		/// </summary>
		public Role() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Role"/> class with a specified role name.
		/// </summary>
		/// <param name="roleName">The name of the role.</param>
		public Role(string roleName) : base(roleName) { }
	}
}