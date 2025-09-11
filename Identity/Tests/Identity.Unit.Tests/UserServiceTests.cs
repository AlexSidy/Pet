using Identity.Api;
using Identity.Api.Services;
using Identity.Api.Services.Interfaces;
using Identity.Unit.Tests.Mocks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.Common.Resources;
using ScanPerson.Models.Responses;

using LoginRequest = ScanPerson.Models.Requests.Auth.LoginRequest;
using RegisterRequest = ScanPerson.Models.Requests.Auth.RegisterRequest;

namespace Identity.Unit.Tests
{
	[TestClass]
	public sealed class UserServiceTests
	{
		// class under tests
		private readonly UserService _cut;

		private readonly Mock<ILogger<UserService>> _logger;
		private readonly Mock<UserManager<User>> _userManager;
		private readonly Mock<IUserStore<User>> _userStoreMock;
		private readonly Mock<ITokenProvider> _tokenProvider;

		private readonly RegisterRequest _registerRequest;
		private readonly LoginRequest _loginRequest;

		public UserServiceTests()
		{
			_logger = new Mock<ILogger<UserService>>();
			_userStoreMock = new Mock<IUserStore<User>>();
			_userManager = MockHelper.GetMockUserManager(_userStoreMock);
			_tokenProvider = new Mock<ITokenProvider>();
			_registerRequest = new RegisterRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			};
			_loginRequest = new LoginRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			};

			_cut = new UserService(_logger.Object, _userManager.Object, _tokenProvider.Object);
		}

		[TestMethod]
		public void Ctor_DependiesAreNotNull_Success()
		{
			// Arrange

			// Act
			var cut = new UserService(_logger.Object, _userManager.Object, _tokenProvider.Object);

			// Assert
			Assert.IsNotNull(cut);
		}

		[TestMethod]
		public async Task RegisterAsync_RequestIsCorrect_ReturnSuccessResult()
		{
			// Arrange
			var expectedResult = new ScanPersonResultResponse<IdentityResult>(IdentityResult.Success);
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
			_userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = (ScanPersonResultResponse<IdentityResult>)await _cut.RegisterAsync(_registerRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsSuccess);
			Assert.IsTrue(result.Result.Succeeded);
		}

		[TestMethod]
		public async Task RegisterAsync_CreateUserIsFail_ReturnFailResult()
		{
			// Arrange
			var expectedError = new IdentityError
			{
				Description = "error"
			};
			var failResult = IdentityResult.Failed([expectedError]);
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
			_userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(failResult);

			// Act
			var result = await _cut.RegisterAsync(_registerRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(expectedError.Description, result.Error);
		}

		[TestMethod]
		public async Task RegisterAsync_UserIsfound_ReturnFailResult()
		{
			// Arrange
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

			// Act
			var result = await _cut.RegisterAsync(_registerRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.UserAlredyExist, result.Error);
		}

		[TestMethod]
		public async Task RegisterAsync_CreateAsyncThrowsError_ReturnFailedResult()
		{
			// Arrange
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
			_userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Throws<InvalidOperationException>();

			// Act
			var result = await _cut.RegisterAsync(_registerRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.ClientOperationError, result.Error);
		}

		[TestMethod]
		public async Task RegisterAsync_FindByEmailThrowsError_ReturnFailedResult()
		{
			// Arrange
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).Throws<InvalidOperationException>();

			// Act
			var result = await _cut.RegisterAsync(_registerRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.ClientOperationError, result.Error);
		}

		[TestMethod]
		public async Task LoginAsync_UserNotFound_ReturnFailResult()
		{
			// Arrange
			var expectedError = new IdentityError
			{
				Description = "error"
			};
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

			// Act
			var result = await _cut.LoginAsync(_loginRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.LoginOrPasswordHasError, result.Error);
		}

		[TestMethod]
		public async Task LoginAsync_PasswordIsIncorrect_ReturnFailResult()
		{
			// Arrange
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
			_userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

			// Act
			var result = await _cut.LoginAsync(_loginRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.LoginOrPasswordHasError, result.Error);
		}

		[TestMethod]
		public async Task LoginAsync_RequestIsCorrect_ReturnSuccessResult()
		{
			// Arrange
			var token = "token";
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
			_userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
			_tokenProvider.Setup(x => x.GenerateTokenAsync(It.IsAny<User>())).ReturnsAsync(token);

			// Act
			var result = (ScanPersonResultResponse<string>)await _cut.LoginAsync(_loginRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual(token, result.Result);
		}

		[TestMethod]
		public async Task LoginAsync_FindByEmailAsyncThrowsException_ReturnFailedResult()
		{
			// Arrange
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).Throws<InvalidOperationException>();


			// Act
			var result = await _cut.LoginAsync(_loginRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.ClientOperationError, result.Error);
		}

		[TestMethod]
		public async Task LoginAsync_CheckPasswordAsyncThrowsException_ReturnFailedResult()
		{
			// Arrange
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
			_userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Throws<InvalidOperationException>();

			// Act
			var result = await _cut.LoginAsync(_loginRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.ClientOperationError, result.Error);
		}

		[TestMethod]
		public async Task LoginAsync_GenerateTokenAsyncThrowsException_ReturnFailedResult()
		{
			// Arrange
			_userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
			_userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
			_tokenProvider.Setup(x => x.GenerateTokenAsync(It.IsAny<User>())).Throws<InvalidOperationException>();

			// Act
			var result = await _cut.LoginAsync(_loginRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.ClientOperationError, result.Error);
		}
	}
}
