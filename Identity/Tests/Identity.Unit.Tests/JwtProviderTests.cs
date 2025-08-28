using System.Security.Claims;

using Identity.Api;
using Identity.Api.Services;
using Identity.Unit.Tests.Mocks;

using Microsoft.AspNetCore.Identity;

using Moq;

using ScanPerson.Models.Options.Auth;

namespace Identity.Unit.Tests
{
	[TestClass]
	public sealed class JwtProviderTests
	{
		// class under tests
		private readonly JwtProvider _cut;

		private readonly JwtOptions jwtOptions;
		private readonly Mock<UserManager<User>> _userManager;
		private readonly Mock<IUserStore<User>> _userStoreMock;

		public JwtProviderTests()
		{
			jwtOptions = new JwtOptions {
				Audience = "test",
				ExpireHours = 10,
				Issuer = "test",
				SecretKey = "mySuperSecretKeyWithSizeGreatherThan256Bit"
			};
			_userStoreMock = new Mock<IUserStore<User>>();
			_userManager = MockHelper.GetMockUserManager(_userStoreMock);

			_cut = new JwtProvider(jwtOptions, _userManager.Object);
		}

		[TestMethod]
		public void Ctor_DependiesAreNotNull_Success()
		{
			// Arrange

			// Act
			var cut = new JwtProvider(jwtOptions, _userManager.Object);

			// Assert
			Assert.IsNotNull(cut);
		}

		[TestMethod]
		public async Task GenerateTokenAsync_RequestIsCorrect_ReturnSuccessResult()
		{
			// Arrange
			_userManager.Setup(x => x.GetClaimsAsync(It.IsAny<User>())).ReturnsAsync([new Claim("test", "test")]);

			// Act
			var result = await _cut.GenerateTokenAsync(new User());

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task GenerateTokenAsync_ThrowWhenGetClaim_Throws()
		{
			// Arrange
			var error = "Error";
			var exceptedError = new InvalidOperationException(error);
			_userManager.Setup(x => x.GetClaimsAsync(It.IsAny<User>())).ThrowsAsync(exceptedError);

			// Act Assert
			var result = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _cut.GenerateTokenAsync(new User()));

			Assert.IsNotNull(result);
			Assert.AreEqual(error, result.Message);
		}
	}
}
