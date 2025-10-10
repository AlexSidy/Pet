using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;
using Moq.Protected;

using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Responses;

using TelegramBots.BusinessLogic.Services;
using TelegramBots.Integration.Tests.Base;

namespace TelegramBots.Integration.Tests;

[TestClass]
public class ScanPersonWebApiServiceTests : IntegrationTestsBase
{
	/// <summary>
	/// CUT.
	/// </summary>
	private readonly ScanPersonWebApiService _cut;

	public ScanPersonWebApiServiceTests()
	{
		_cut = Factory.Services.GetRequiredService<ScanPersonWebApiService>();
	}

	[TestMethod]
	public async Task GetDataAsync_ApiReturnsSuccess_ReturnsCorrectData()
	{
		// Arrange
		var expectedPhoneNumber = "9995556677";
		var apiMethodName = "GetScanPersonInfoAsync";

		var apiSuccessResponse = new ScanPersonResultResponse<PersonInfoItem>
		{
			IsSuccess = true,
			Result = new PersonInfoItem { Id = 1, Names = ["Test User"], Location = new LocationItem { OperatorCity = "City", CountryName = "Country" } }
		};

		MockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(JsonSerializer.Serialize(apiSuccessResponse), Encoding.UTF8, "application/json")
			});

		// Act
		var result = await _cut.GetDataAsync(expectedPhoneNumber, TestContext.CancellationTokenSource.Token);

		// Assert
		MockHttpMessageHandler.Protected().Verify(
			"SendAsync",
			Times.Once(),
			ItExpr.Is<HttpRequestMessage>(req =>
				req.Method == HttpMethod.Post &&
				req.RequestUri!.AbsoluteUri.Contains(apiMethodName)),
			ItExpr.IsAny<CancellationToken>()
		);

		Assert.IsNotNull(result);
		Assert.Contains("Test User", result);
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
	public async Task GetDataAsync_ApiReturnsError_ReturnsClientOperationError()
	{
		// Arrange
		var expectedPhoneNumber = "9995556677";

		MockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.InternalServerError,
				Content = new StringContent("Server crashed")
			});

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

	[TestMethod]
	public async Task GetDataAsync_ApiThrows_ReturnsClientOperationError()
	{
		// Arrange
		var expectedPhoneNumber = "9995556677";

		MockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.Throws<Exception>();

		// Act
		var result = await _cut.GetDataAsync(expectedPhoneNumber, TestContext.CancellationTokenSource.Token);

		// Assert
		Assert.AreEqual(Messages.ClientOperationError, result);

		// Проверяем, что логгер был вызван с ошибкой
		MockWebApiLogger.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => true), // Проверяем само сообщение об ошибке (опущено для краткости)
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}
}