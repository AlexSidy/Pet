using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ScanPerson.Auth.Api.Services.Interfaces;
using ScanPerson.Models.Contracts.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScanPerson.Auth.Api.Services
{
	public class JwtProvider(JwtOptions options, UserManager<User> userManager) : ITokenProvider
	{
		public async Task<string> GenerateTokenAsync(User user)
		{
			var claims = await userManager.GetClaimsAsync(user);
			if (!claims.Any())
			{
				claims = [
					new Claim(JwtRegisteredClaimNames.Email, user.Email!),
					new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString())
				];
			}

			var jwt = new JwtSecurityToken(
					issuer: options.Issuer,
					audience: options.Audience,
					claims: claims,
					expires: DateTime.UtcNow.Add(TimeSpan.FromHours(options.ExpireHours)),
					signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)), SecurityAlgorithms.HmacSha256));

			return new JwtSecurityTokenHandler().WriteToken(jwt);
		}
	}
}
