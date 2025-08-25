using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.Common.Resources;
using ScanPerson.Common.Tests;
using ScanPerson.Models.Contracts;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.Unit.Tests
{
	[TestClass]
	public sealed class GeoServiceTests
	{
		// class under tests
		private readonly GeoService _cut;

		private readonly Mock<ILogger<GeoService>> _logger;
		private Mock<IHttpClientFactory> _httpClientFactory;
		private readonly ScanPersonSecrets _secrets = new() { HtmlWebRuApiKey = "key" };
		private readonly ServicesOptions _servicesOptions = new() { UnUsingServices = ["TestUnusedService", "TestService"] };

		public GeoServiceTests()
		{
			_logger = new Mock<ILogger<GeoService>>();
			_httpClientFactory = new Mock<IHttpClientFactory>();
			_httpClientFactory.MockHttpClientFactoryWithSuccessResponse();

			_cut = new GeoService(_logger.Object, _httpClientFactory.Object, _secrets, _servicesOptions);
		}

		[TestMethod]
		public void Ctor_DependiesAreNotNull_Success()
		{
			// Arrange

			// Act
			var cut = new GeoService(_logger.Object, _httpClientFactory.Object, _secrets, _servicesOptions);

			// Assert
			Assert.IsNotNull(cut);
		}

		[TestMethod]
		public async Task GetInfoAsync_PersonRequestIsCorrect_ReturnSuccessResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest();
			var expectedResult = CreationHelper.GetPersonResponse();

			// Act
			var result = (ScanPersonResultResponse<PersonInfoItem>)await _cut.GetInfoAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsSuccess);
			AssertHelper.AssertLocationResult(expectedResult[0], result);
		}

		[TestMethod]
		public async Task GetInfoAsync_ResponseFromHttpIsNotSuccess_ReturnFailResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest();
			_httpClientFactory.Reset();
			_httpClientFactory.MockHttpClientFactoryWithErrorResponse();

			var cut = new GeoService(_logger.Object, _httpClientFactory.Object, _secrets, _servicesOptions);

			// Act
			var result = await cut.GetInfoAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.ClientOperationError, result.Error);
		}

		[TestMethod]
		public void CanAccept_ThisNotInUnUsingServices_ReturnTrue()
		{
			// Arrange

			// Act
			var result = _cut.CanAccept();

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void CanAccept_ThisInUnUsingServices_ReturnTrue()
		{
			// Arrange
			var servicesOptions = new ServicesOptions() { UnUsingServices = ["GeoService", "TestUnusedService"] };

			// Act
			var cut = new GeoService(_logger.Object, _httpClientFactory.Object, _secrets, servicesOptions);
			var result = cut.CanAccept();

			// Assert
			Assert.IsFalse(result);
		}
	}
}
