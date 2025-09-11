using AutoMapper;

using Moq;
using Moq.Protected;

using ScanPerson.Models.Items;

namespace ScanPerson.Common.Tests
{
	/// <summary>
	/// Вспомогательный класс для настройки Mock объектов.
	/// </summary>
	public static class MockHelper
	{

		/// <summary>
		/// Настраивает общий Get метод Mock объект IHttpClientFactory для получения успешного ответа.
		/// </summary>
		public static void SetupHttpClientFactoryWithSuccessResponse(this Mock<IHttpClientFactory> httpClientFactory)
		{
			var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			mockHttpMessageHandler
				.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(CreationHelper.GetSuccessHttpMessage());

			var httpClient = new HttpClient(mockHttpMessageHandler.Object);
			httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
		}

		/// <summary>
		/// Настраивает общий Get метод Mock объект IHttpClientFactory для получения неуспешного ответа.
		/// </summary>
		public static void SetupHttpClientFactoryWithErrorResponse(this Mock<IHttpClientFactory> httpClientFactory)
		{
			var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			mockHttpMessageHandler
				.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(CreationHelper.GetInternalServerErrorHttpMessage());

			var httpClient = new HttpClient(mockHttpMessageHandler.Object);
			httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
		}

		public static void SetupAutoMapper(this Mock<IMapper> mapper)
		{
			mapper.Setup(x => x.Map<LocationItem>(It.IsAny<LocationDeserialized>()))
				.Returns(CreationHelper.GetPersonResponse()[0].Result.Location);
		}
	}
}
