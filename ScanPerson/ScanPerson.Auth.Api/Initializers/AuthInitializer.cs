using Microsoft.AspNetCore.Identity;
using ScanPerson.Auth.Api.Initializers.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ScanPerson.Auth.Api.Initializers;

internal class AuthInitializer(
		UserManager<User> userManager,
		RoleManager<Role> roleManager) : IInitializer
{
	public async Task SeedAsync()
	{
		try
		{
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
					if(!createResult.Succeeded)
					{
						throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(x => x.Description)));
					}
					found = await userManager.FindByEmailAsync(user.mail);
				}

				if (found == null)
				{
					return;
				}

				var roles = await userManager.GetRolesAsync(found);
				if (!roles.Any())
				{
					// add role to user
					await userManager.AddToRoleAsync(found, user.role);
				}

				var claims = await userManager.GetClaimsAsync(found) ?? new List<Claim>();
				if (!claims.Any())
				{
					// add claim to user
					var claimType = "Permission";
					claims.Add(new Claim(JwtRegisteredClaimNames.Email, found.Email!));
					claims.Add(new Claim(JwtRegisteredClaimNames.Sid, found.Id.ToString()));
					claims.Add(new Claim(claimType, user.claim));
					foreach (var role in roles)
					{
						claims.Add(new Claim("Role", role));
					}
					
					await userManager.AddClaimsAsync(found, claims);
				}
			}
		}
		catch (Exception ex)
		{
			//TODO : in task #24 add logging
			throw ex;
		}
	}
}