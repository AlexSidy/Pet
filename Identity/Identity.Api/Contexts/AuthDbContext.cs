using Identity.Api.Migrations._2024_12;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api
{
	public class AuthDbContext : IdentityDbContext<User, Role, long>
	{
		public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.HasDefaultSchema(InitiaIdentity.AuthSchema);
			base.OnModelCreating(builder);
		}
	}

	public class User : IdentityUser<long>
	{
		public User() : base() { }
	}
	public class Role : IdentityRole<long>
	{
		public Role() : base() { }
		public Role(string roleName) : base(roleName) { }
	}
}
