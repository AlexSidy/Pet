using System.Reflection;

using Identity.Api.Services.Interfaces;

using Microsoft.AspNetCore.Identity;

using ScanPerson.Common.Operations.Base;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Requests.Auth;
using ScanPerson.Models.Responses;

namespace Identity.Api.Services
{
	public class UserService(
		ILogger<UserService> logger,
		UserManager<User> userManager,
		ITokenProvider jwtProvider) : OperationBase, IUserService
	{
		public async Task<ScanPersonResponseBase> RegisterAsync(RegisterRequest request)
		{
			try
			{
				logger.LogInformation(Messages.StartedMethod, MethodBase.GetCurrentMethod());

				var found = await userManager.FindByEmailAsync(request.Email);
				if (found == null)
				{
					var newUser = new User { UserName = request.Email, Email = request.Email, SecurityStamp = Guid.NewGuid().ToString() };
					var createResult = await userManager.CreateAsync(newUser, request.Password);
					if (createResult.Succeeded)
					{
						return GetSuccess(createResult);
					}

					return GetFail(string.Join(", ", createResult.Errors.Select(x => x.Description)));
				}

				return GetFail(Messages.UserAlredyExist);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, Messages.OperationError, GetType().Name);

				return GetFail(Messages.ClientOperationError);
			}
		}

		public async Task<ScanPersonResponseBase> LoginAsync(LoginRequest request)
		{
			try
			{
				logger.LogInformation(Messages.StartedMethod, MethodBase.GetCurrentMethod());
				var found = await userManager.FindByEmailAsync(request.Email);
				if (found == null)
				{
					return GetFail(Messages.LoginOrPasswordHasError);
				}

				var isVerify = await userManager.CheckPasswordAsync(found, request.Password);
				if (isVerify)
				{
					var jwt = await jwtProvider.GenerateTokenAsync(found);

					return GetSuccess(jwt);
				}

				return GetFail(Messages.LoginOrPasswordHasError);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, Messages.OperationError, GetType().Name);

				return GetFail(Messages.ClientOperationError);
			}
		}
	}
}
