using Microsoft.AspNetCore.Mvc;
using ScanPerson.Models.Requests.Auth;
using ScanPerson.Auth.Api.Services.Interfaces;

namespace ScanPerson.Auth.Api.Controllers
{
	[ApiController]
	[Route(Program.AuthApi + "/[controller]")]
	public class AuthController(
		//TODO: in task #24 add logging
		ILogger<AuthController> logger,
		IUserService userService
		) : AuthControllerBase
	{
		[HttpPost(nameof(RegisterAsync))]
		public async Task<IResult> RegisterAsync([FromBody] RegisterRequest request)
		{
			return GetResult(await userService.RegisterAsync(request));
		}

		[HttpPost(nameof(LoginAsync))]
		public async Task<IResult> LoginAsync([FromBody] LoginRequest request)
		{
			return GetResult(await userService.LoginAsync(request), null, Results.Unauthorized());
		}
	}
}
