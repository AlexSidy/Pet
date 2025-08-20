using Microsoft.AspNetCore.Mvc;

using ScanPerson.Auth.Api.Services.Interfaces;
using ScanPerson.Common.Controllers;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Requests.Auth;

namespace ScanPerson.Auth.Api.Controllers
{
	[ApiController]
	[Route(Program.AuthApi + "/[controller]")]
	public class AuthController(
		ILogger<AuthController> logger,
		IUserService userService
		) : ScanPersonControllerBase
	{
		[HttpPost(nameof(RegisterAsync))]
		public async Task<IResult> RegisterAsync([FromBody] RegisterRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, ControllerContext?.RouteData?.Values["action"]);
			return GetResult(await userService.RegisterAsync(request));
		}

		[HttpPost(nameof(LoginAsync))]
		public async Task<IResult> LoginAsync([FromBody] LoginRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, ControllerContext?.RouteData?.Values["action"]);
			return GetResult(await userService.LoginAsync(request), null, Results.Unauthorized());
		}
	}
}
