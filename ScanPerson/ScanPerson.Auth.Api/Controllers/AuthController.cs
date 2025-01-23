using Microsoft.AspNetCore.Mvc;
using ScanPerson.Models.Requests.Auth;
using ScanPerson.Auth.Api.Services.Interfaces;

namespace ScanPerson.Auth.Api.Controllers
{
	[ApiController]
	[Route("authApi/[controller]")]
	public class AuthController(
		//TODO в задаче #24 добавить логирование
		ILogger<AuthController> logger,
		IUserService userService
		) : AuthControllerBase
	{
		[HttpPost]
		[Route("register")]
		public async Task<IResult> RegisterAsync([FromBody] RegisterRequest request)
		{
			return GetResult(await userService.RegisterAsync(request));
		}

		[HttpPost]
		[Route("login")]
		public async Task<IResult> LoginAsync(
			[FromBody] LoginRequest request,
			[FromServices] ITokenProvider jwtProvider)
		{
			return GetResult(await userService.LoginAsync(request), null, Results.Unauthorized());
		}
	}
}
