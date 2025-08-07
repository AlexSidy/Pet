using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ScanPerson.Auth.Api.Migrations._2024_12;

namespace ScanPerson.Auth.Api
{
	public class AuthDbContext : IdentityDbContext<User, Role, long>
	{
		public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema(InitiaIdentity.AuthSchema);
			base.OnModelCreating(modelBuilder);
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
