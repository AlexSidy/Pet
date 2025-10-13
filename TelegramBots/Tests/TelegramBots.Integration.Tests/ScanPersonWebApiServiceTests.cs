using Grpc.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.Common.GrpcServices;
using ScanPerson.Common.Resources;

using TelegramBots.BusinessLogic.Services;
using TelegramBots.BusinessLogic.Services.Interfaces;
using TelegramBots.Integration.Tests.Base;

namespace TelegramBots.Integration.Tests
{
	[TestClass]
	public class ScanPersonWebApiServiceTests : IntegrationTestsBase
	{
		/// <summary>
		/// CUT.
		/// </summary>
		private readonly IApiService _cut;

		public ScanPersonWebApiServiceTests()
		{
			_cut = Factory.Services.GetRequiredService<IApiService>();
		}

		[TestMethod]
		public async Task GetDataAsync_ApiReturnsSuccess_ReturnsCorrectData()
		{
			// Arrange
			var expectedPhoneNumber = "9995556677";
			var request = new GrpcPersonInfoRequest { PhoneNumber = expectedPhoneNumber };
			var apiSuccessResponse = new GrpcScanPersonResponse
			{
				IsSuccess = true,
				Result = new GrpcPersonInfoItem { Id = 1, Location = new GrpcLocationItem { OperatorCity = "City", CountryName = "Country" } }
			};
			var asyncResponse = new AsyncUnaryCall<GrpcScanPersonResponse>(Task.FromResult(apiSuccessResponse), null, null, null, null);
			var token = TestContext.CancellationTokenSource.Token;
			MockGrpcClient.Setup(x => x.GetGrpcScanPersonInfoAsync(request, It.IsAny<Metadata>(), It.IsAny<DateTime?>(), token))
				.Returns(asyncResponse);

			// Act
			var result = await _cut.GetDataAsync(expectedPhoneNumber, TestContext.CancellationTokenSource.Token);

			// Assert
			Assert.IsNotNull(result);
			Assert.Contains("City", result);
			Assert.Contains("Country", result);
		}

		[TestMethod]
		public async Task GetDataAsync_PhoneNumberIsNotValid_ReturnsValidationError()
		{
			// Arrange
			var expectedPhoneNumber = "123";

			// Act
			var result = await _cut.GetDataAsync(expectedPhoneNumber, TestContext.CancellationTokenSource.Token);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(ScanPersonWebApiService.ValidationError, result);
		}

		[TestMethod]
		public async Task GetDataAsync_ApiReturnsErrorResponse_ReturnsClientOperationError()
		{
			// Arrange
			var expectedPhoneNumber = "9995556677";
			var request = new GrpcPersonInfoRequest { PhoneNumber = expectedPhoneNumber };
			var apiSuccessResponse = new GrpcScanPersonResponse
			{
				IsSuccess = false,
				Error = "Error"
			};
			var asyncResponse = new AsyncUnaryCall<GrpcScanPersonResponse>(Task.FromResult(apiSuccessResponse), null, null, null, null);
			var token = TestContext.CancellationTokenSource.Token;
			MockGrpcClient.Setup(x => x.GetGrpcScanPersonInfoAsync(request, It.IsAny<Metadata>(), It.IsAny<DateTime?>(), token))
				.Returns(asyncResponse);

			// Act
			var result = await _cut.GetDataAsync(expectedPhoneNumber, TestContext.CancellationTokenSource.Token);

			// Assert
			Assert.AreEqual(Messages.ClientOperationError, result);

			MockWebApiLogger.Verify(
				x => x.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(apiSuccessResponse.Error)),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}

		[TestMethod]
		public async Task GetDataAsync_ApiReturnsNullResponse_ReturnsClientOperationError()
		{
			// Arrange
			var expectedPhoneNumber = "9995556677";
			var request = new GrpcPersonInfoRequest { PhoneNumber = expectedPhoneNumber };
			var apiSuccessResponse = new GrpcScanPersonResponse
			{
				IsSuccess = false,
				Error = "Error"
			};
			AsyncUnaryCall<GrpcScanPersonResponse>? asyncResponse = null;
			var token = TestContext.CancellationTokenSource.Token;
			MockGrpcClient.Setup(x => x.GetGrpcScanPersonInfoAsync(request, It.IsAny<Metadata>(), It.IsAny<DateTime?>(), token))
				.Returns(asyncResponse);

			// Act
			var result = await _cut.GetDataAsync(expectedPhoneNumber, TestContext.CancellationTokenSource.Token);

			// Assert
			Assert.AreEqual(Messages.ClientOperationError, result);

			MockWebApiLogger.Verify(
				x => x.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => true),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}

		[TestMethod]
		public async Task GetDataAsync_ApiThrows_ReturnsClientOperationError()
		{
			// Arrange
			var expectedPhoneNumber = "9995556677";
			var request = new GrpcPersonInfoRequest { PhoneNumber = expectedPhoneNumber };
			var token = TestContext.CancellationTokenSource.Token;
			MockGrpcClient.Setup(x => x.GetGrpcScanPersonInfoAsync(request, It.IsAny<Metadata>(), It.IsAny<DateTime?>(), token))
				.Throws<InvalidOperationException>();

			// Act
			var result = await _cut.GetDataAsync(expectedPhoneNumber, TestContext.CancellationTokenSource.Token);

			// Assert
			Assert.AreEqual(Messages.ClientOperationError, result);

			MockWebApiLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => true),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}
	}
}