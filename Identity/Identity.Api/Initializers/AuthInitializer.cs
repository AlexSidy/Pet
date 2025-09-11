using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

using Identity.Api.Initializers.Interfaces;

using Microsoft.AspNetCore.Identity;

using ScanPerson.Common.Resources;

namespace Identity.Api.Initializers;

/// <summary>
/// Initializes authentication-related data such as roles and users.
/// </summary>
internal class AuthInitializer(
		ILogger<AuthInitializer> logger,
		UserManager<User> userManager,
		RoleManager<Role> roleManager) : IInitializer
{
	/// <summary>
	/// Seeds initial data into the authentication system.
	/// This includes creating roles ('admin', 'user') and predefined users with associated roles and claims.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown if user creation fails.</exception>
	public async Task SeedAsync()
	{
		try
		{
			logger.LogInformation(Messages.StartedMethod, MethodBase.GetCurrentMethod());

			// Define and create roles if they don't exist
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

			// Define users with their properties: name, email, role, claim, and password
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
					// Create new user if not found
					var newUser = new User { UserName = user.name, Email = user.mail, SecurityStamp = Guid.NewGuid().ToString() };
					var createResult = await userManager.CreateAsync(newUser, user.pass);
					if (!createResult.Succeeded)
					{
						throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(x => x.Description)));
					}
					found = await userManager.FindByEmailAsync(user.mail);
				}

				if (found == null)
				{
					return; // Should not happen if user creation succeeded, but a safety check
				}

				// Add roles and claims to the user
				IList<string> roles = await AddRole(user.role, found);
				await AddClaim(user.claim, found, roles);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, Messages.InitDataError);
			// TODO: Add transaction rollback #35
		}
	}

	/// <summary>
	/// Assigns a role to a user if they do not already have any roles.
	/// </summary>
	/// <param name="role">The name of the role to assign.</param>
	/// <param name="found">The user to whom the role will be assigned.</param>
	/// <returns>A list of existing roles for the user.</returns>
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
	/// Adds a claim to a user if they do not already have specific claims.
	/// This method also adds standard claims like Email, User ID (Sid), and Role.
	/// </summary>
	/// <param name="claim">The name of the specific claim to add (e.g., "CanEdit", "CanView").</param>
	/// <param name="found">The user to whom the claim will be added.</param>
	/// <param name="roles">The roles associated with the user, used to add "Role" claims.</param>
	private async Task AddClaim(string claim, User found, IList<string> roles)
	{
		var claims = await userManager.GetClaimsAsync(found) ?? new List<Claim>();
		if (!claims.Any())
		{
			// add claim to user
			var claimType = "Permission"; // Custom claim type for permissions
			claims.Add(new Claim(JwtRegisteredClaimNames.Email, found.Email!)); // Add email as a JWT claim
			claims.Add(new Claim(JwtRegisteredClaimNames.Sid, found.Id.ToString())); // Add user ID as a JWT claim
			claims.Add(new Claim(claimType, claim)); // Add the specific permission claim

			// Add claims for each of the user's roles
			foreach (var role in roles)
			{
				claims.Add(new Claim("Role", role));
			}

			await userManager.AddClaimsAsync(found, claims);
		}
	}
}