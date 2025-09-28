using AutoMapper;

using Microsoft.Extensions.Logging;

using Moq;
using Moq.Protected;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.Common.Tests;
using ScanPerson.Models.Items;
using ScanPerson.Models.Options;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.Unit.Tests
{
	[TestClass]
	public sealed class BrowserBotServiceTests
	{
		// class under tests
		private readonly BrowserBotService _cut;

		private readonly Mock<ILogger<BrowserBotService>> _logger;
		private Mock<IHttpClientFactory> _httpClientFactory;
		private readonly ScanPersonSecrets _secrets = new() { HtmlWebRuApiKey = "key" };
		private readonly ServicesOptions _servicesOptions;
		private readonly Mock<IMapper> _mapper;


		public BrowserBotServiceTests()
		{
			_logger = new Mock<ILogger<BrowserBotService>>();
			_httpClientFactory = new Mock<IHttpClientFactory>();
			_httpClientFactory.SetupHttpClientFactoryWithSuccessResponse();
			_servicesOptions = new()
			{
				UnUsingServices = ["TestUnusedService", "TestService"],
				GeoServiceOptions = new GeoServiceOptions
				{
					BaseUrl = "https://test.ru/geo"
				},
				BrowserBotServiceOptions = new BrowserBotServiceOptions
				{
					ServiceHostOptions = new ServiceHostOptions
					{
						Host = "browserbot",
						Port = 443,
						ApiVersion = "Api",
						ControllerName = "Controller"
					}
				}
			};
			_mapper = new Mock<IMapper>();
			_mapper.SetupAutoMapper();

			_cut = new BrowserBotService(_logger.Object, _httpClientFactory.Object, _secrets, _servicesOptions, _mapper.Object);
		}

		[TestMethod]
		public void Ctor_DependiesAreNotNull_Success()
		{
			// Arrange

			// Act
			var cut = new BrowserBotService(_logger.Object, _httpClientFactory.Object, _secrets, _servicesOptions, _mapper.Object);

			// Assert
			Assert.IsNotNull(cut);
		}

		[TestMethod]
		public async Task GetInfoAsync_PersonRequestIsCorrect_ReturnSuccessResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest { PhoneNumber = "12345" };
			_httpClientFactory.Reset();
			var response1 = @"
			{
			  ""result"": ""Test name1"",
			  ""isSuccess"": true,
			  ""error"": null
			}";
			var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			mockHttpMessageHandler
				.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString().Contains("GetNameByPhoneNumberAsync")),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(CreationHelper.GetSuccessHttpMessage(response1));

			var response2 = @"
			{
			  ""result"": [
			    ""Test name2""
			  ],
			  ""isSuccess"": true,
			  ""error"": null
			}";
			mockHttpMessageHandler
				.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri!.ToString().Contains("GetNamesByPhoneNumberAsync")),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(CreationHelper.GetSuccessHttpMessage(response2));

			var httpClient = new HttpClient(mockHttpMessageHandler.Object);

			_httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
			_mapper.Setup(x => x.Map<PersonInfoRequest>(It.IsAny<ServiceRequest>())).Returns(personRequest);

			// Act
			var cut = new BrowserBotService(_logger.Object, _httpClientFactory.Object, _secrets, _servicesOptions, _mapper.Object);
			var result = (ScanPersonResultResponse<PersonInfoItem>)await cut.GetInfoAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("Test name1", result.Result.Names[0]);
			Assert.AreEqual("Test name2", result.Result.Names[1]);
		}

		[TestMethod]
		public async Task GetInfoAsync_ResponseFromHttpIsNotSuccess_ReturnFailResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest();
			_mapper.Setup(x => x.Map<PersonInfoRequest>(It.IsAny<ServiceRequest>())).Returns(personRequest);
			_httpClientFactory.Reset();
			_httpClientFactory.SetupHttpClientFactoryWithErrorResponse();

			var cut = new BrowserBotService(_logger.Object, _httpClientFactory.Object, _secrets, _servicesOptions, _mapper.Object);

			// Act
			var result = await cut.GetInfoAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.Contains("Internal server error", result.Error);
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
			var servicesOptions = new ServicesOptions() { UnUsingServices = ["BrowserBotService", "TestUnusedService"] };

			// Act
			var cut = new BrowserBotService(_logger.Object, _httpClientFactory.Object, _secrets, servicesOptions, _mapper.Object);
			var result = cut.CanAccept();

			// Assert
			Assert.IsFalse(result);
		}
	}
}
