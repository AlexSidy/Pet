using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

using Identity.Api.Initializers.Interfaces;

using Microsoft.AspNetCore.Identity;

using ScanPerson.Common.Resources;

namespace Identity.Api.Initializers;

internal class AuthInitializer(
		ILogger<AuthInitializer> logger,
		UserManager<User> userManager,
		RoleManager<Role> roleManager) : IInitializer
{
	public async Task SeedAsync()
	{
		try
		{
			logger.LogInformation(Messages.StartedMethod, MethodBase.GetCurrentMethod());
			string[] roleNames = { "admin", "user" };
			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					// create role
					await roleManager.CreateAsync(new Role(roleName));
				}
			}

			var users = new (string name, string mail, string role, string claim, string pass)[]
			{
				new ("admin", "admin@example.com", "admin", "CanEdit", "Admin123!"),
				new ("user", "user@example.com", "user", "CanView", "User123!")
			};

			foreach (var user in users)
			{
				var found = await userManager.FindByEmailAsync(user.mail);
				if (found == null)
				{
					var newUser = new User { UserName = user.name, Email = user.mail, SecurityStamp = Guid.NewGuid().ToString() };
					// create user
					var createResult = await userManager.CreateAsync(newUser, user.pass);
					if (!createResult.Succeeded)
					{
						throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(x => x.Description)));
					}
					found = await userManager.FindByEmailAsync(user.mail);
				}

				if (found == null)
				{
					return;
				}

				IList<string> roles = await AddRole(user.role, found);

				await AddClaim(user.claim, found, roles);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, Messages.InitDataError);
			// Add transaction rollback #35
		}
	}

	/// <summary>
	/// Add role if user havent any role.
	/// </summary>
	/// <param name="role">Role name.</param>
	/// <param name="found">Found user.</param>
	private async Task<IList<string>> AddRole(string role, User found)
	{
		var roles = await userManager.GetRolesAsync(found);
		if (!roles.Any())
		{
			// add role to user
			await userManager.AddToRoleAsync(found, role);
		}

		return roles;
	}

	/// <summary>
	/// Add claim if user havent this.
	/// </summary>
	/// <param name="claim">Claim name.</param>
	/// <param name="found">Found user.</param>
	/// <param name="roles">Roles.</param>
	private async Task AddClaim(string claim, User found, IList<string> roles)
	{
		var claims = await userManager.GetClaimsAsync(found) ?? new List<Claim>();
		if (!claims.Any())
		{
			// add claim to user
			var claimType = "Permission";
			claims.Add(new Claim(JwtRegisteredClaimNames.Email, found.Email!));
			claims.Add(new Claim(JwtRegisteredClaimNames.Sid, found.Id.ToString()));
			claims.Add(new Claim(claimType, claim));
			foreach (var role in roles)
			{
				claims.Add(new Claim("Role", role));
			}

			await userManager.AddClaimsAsync(found, claims);
		}
	}
}