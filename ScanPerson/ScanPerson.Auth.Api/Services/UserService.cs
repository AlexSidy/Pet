using Microsoft.AspNetCore.Identity;
using ScanPerson.Auth.Api.Controllers;
using ScanPerson.Auth.Api.Resources;
using ScanPerson.Auth.Api.Services.Base;
using ScanPerson.Auth.Api.Services.Interfaces;
using ScanPerson.Models.Requests.Auth;
using ScanPerson.Models.Responses;

namespace ScanPerson.Auth.Api.Services
{
	public class UserService(
		//TODO в задаче #24 добавить логирование
		ILogger<AuthController> logger,
		UserManager<User> userManager,
		ITokenProvider jwtProvider) : OperationBase, IUserService
	{
		public async Task<ScanPersonResponse> RegisterAsync(RegisterRequest request)
		{
			var found = await userManager.FindByEmailAsync(request.Email);
			if (found == null)
			{
				var newUser = new User { UserName = request.Email, Email = request.Email, SecurityStamp = Guid.NewGuid().ToString() };
				var createResult = await userManager.CreateAsync(newUser, request.Password);
				if (createResult.Succeeded)
				{
					return GetSuccess();
				}

				return GetFail(string.Join(", ", createResult.Errors.Select(x => x.Description)));
			}

			return GetFail(Messages.UserAlredyExist);
		}

		public async Task<ScanPersonResponse> LoginAsync(LoginRequest request)
		{
			var found = await userManager.FindByEmailAsync(request.Email);
			if (found == null)
			{
				return GetFail();
			}

			var isVerify = await userManager.CheckPasswordAsync(found, request.Password);
			if (!isVerify)
			{
				return GetFail();
			}

			var jwt = await jwtProvider.GenerateTokenAsync(found);

			return GetSuccess(jwt);
		}
	}
}
