using Identity.Api.Controllers;
using Identity.Api.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.Models.Requests.Auth;
using ScanPerson.Models.Responses;

namespace Identity.Unit.Tests
{
	[TestClass]
	public sealed class AuthControllerTests
	{
		// class under tests
		private readonly AuthController _cut;

		private readonly Mock<ILogger<AuthController>> _logger;
		private readonly Mock<IUserService> _userService;

		public AuthControllerTests()
		{
			_logger = new Mock<ILogger<AuthController>>();
			_userService = new Mock<IUserService>();
			_cut = new AuthController(_logger.Object, _userService.Object);
		}

		[TestMethod]
		public void Ctor_DependiesAreNotNull_Success()
		{
			// Arrange

			// Act
			var cut = new AuthController(_logger.Object, _userService.Object);

			// Assert
			Assert.IsNotNull(cut);
		}

		[TestMethod]
		public async Task RegisterAsync_RequestIsCorrect_ReturnSeccessResult()
		{
			// Arrange
			var request = new RegisterRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			};
			var expectedResponse = new ScanPersonResultResponse<IdentityResult>(IdentityResult.Success);
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(expectedResponse);
			_userService.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>())).Returns(taskResponse);

			// Act
			var test = await _cut.RegisterAsync(request);
			var response = (Microsoft.AspNetCore.Http.HttpResults.Ok<ScanPersonResponseBase>)await _cut.RegisterAsync(request);

			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
			var result = (ScanPersonResultResponse<IdentityResult>)response.Value!;
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(result.Error);
		}

		[TestMethod]
		public async Task RegisterAsync_UserServiceReturnErrorResult_ReturnFailResult()
		{
			// Arrange
			var request = new RegisterRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			};
			var errorMessage = "error";
			var expectedResponse = new ScanPersonResultResponse<IdentityResult>([errorMessage]);
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(expectedResponse);
			_userService.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>())).Returns(taskResponse);

			// Act
			var result = (Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>)await _cut.RegisterAsync(request);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
			Assert.IsNotNull(result.Value);
			Assert.AreEqual(errorMessage, result.Value);
		}

		[TestMethod]
		public async Task LoginAsync_RequestIsCorrect_ReturnSeccessResult()
		{
			// Arrange
			var request = new LoginRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			};
			var expectedResponse = new ScanPersonResultResponse<IdentityResult>(IdentityResult.Success);
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(expectedResponse);
			_userService.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>())).Returns(taskResponse);

			// Act
			var test = await _cut.LoginAsync(request);
			var response = (Microsoft.AspNetCore.Http.HttpResults.Ok<ScanPersonResponseBase>)await _cut.LoginAsync(request);

			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
			var result = (ScanPersonResultResponse<IdentityResult>)response.Value!;
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(result.Error);
		}

		[TestMethod]
		public async Task LoginAsync_UserServiceReturnErrorResult_ReturnFailResult()
		{
			// Arrange
			var request = new LoginRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			};
			var errorMessage = "error";
			var expectedResponse = new ScanPersonResultResponse<IdentityResult>([errorMessage]);
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(expectedResponse);
			_userService.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>())).Returns(taskResponse);

			// Act
			var result = (Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult)await _cut.LoginAsync(request);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
		}
	}
}
