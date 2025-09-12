using Identity.Api.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using ScanPerson.Common.Controllers;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Requests.Auth;

namespace Identity.Api.Controllers
{
	/// <summary>
	/// API controller for authentication-related operations, such as user registration and login.
	/// </summary>
	[ApiController]
	[Route(Program.AuthApi + "/[controller]")]
	public class AuthController(
		ILogger<AuthController> logger,
		IUserService userService
		) : ScanPersonControllerBase
	{
		/// <summary>
		/// Registers a new user with the provided details.
		/// </summary>
		/// <param name="request">The registration request containing user details.</param>
		/// <returns>An <see cref="IResult"/> representing the result of the registration attempt.</returns>
		[HttpPost(nameof(RegisterAsync))]
		public async Task<IResult> RegisterAsync([FromBody] RegisterRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, ControllerContext?.RouteData?.Values["action"]);
			return GetResult(await userService.RegisterAsync(request));
		}

		/// <summary>
		/// Authenticates a user and generates a token upon successful login.
		/// </summary>
		/// <param name="request">The login request containing user credentials.</param>
		/// <returns>An <see cref="IResult"/> representing the result of the login attempt.</returns>
		[HttpPost(nameof(LoginAsync))]
		public async Task<IResult> LoginAsync([FromBody] LoginRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, ControllerContext?.RouteData?.Values["action"]);
			return GetResult(await userService.LoginAsync(request), null, Results.Unauthorized());
		}
	}
}