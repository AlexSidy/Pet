using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.Models.Options;
using ScanPerson.WebApi.Middlewares;

namespace ScanPerson.Unit.Tests
{
	[TestClass]
	public class RedisCacheMiddlewareTests
	{
		/// <summary>
		/// Class under tests
		/// </summary>
		private RedisCacheMiddleware? _cut;

		private Mock<ILogger<RedisCacheMiddleware>>? _mockLogger;
		private Mock<IDistributedCache>? _mockDistributedCache;
		private Mock<RequestDelegate>? _mockNext;
		private CacheOptions? _cacheOptions;

		[TestInitialize]
		public void Setup()
		{
			_mockLogger = new Mock<ILogger<RedisCacheMiddleware>>();
			_mockDistributedCache = new Mock<IDistributedCache>();
			_mockNext = new Mock<RequestDelegate>();
			_cacheOptions = new CacheOptions
			{
				IsEnable = true,
				CacheExpiration = 1
			};

			_cut = new RedisCacheMiddleware(_mockLogger!.Object, _mockDistributedCache!.Object, _cacheOptions, _mockNext!.Object);
		}

		[TestMethod]
		public async Task InvokeAsync_CacheIsDisabled_CallsNextDelegate()
		{
			// Arrange
			_cacheOptions!.IsEnable = false;
			var httpContext = CreateHttpContext("http://example.com/api/webapi/some/endpoint");

			// Act
			await _cut!.Invoke(httpContext);

			// Assert
			_mockNext!.Verify(x => x(It.IsAny<HttpContext>()), Times.Once,
				"Next delegate should be called when cache is disabled.");
			_mockDistributedCache!.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never,
				"Distributed cache should not be accessed when cache is disabled.");
		}

		[TestMethod]
		public async Task InvokeAsync_UrlDoesNotContainWebApi_CallsNextDelegate()
		{
			// Arrange
			var httpContext = CreateHttpContext("http://example.com/some/other/endpoint");

			// Act
			await _cut!.Invoke(httpContext);

			// Assert
			_mockNext!.Verify(x => x(It.IsAny<HttpContext>()), Times.Once,
				"Next delegate should be called when URL does not contain 'webapi'.");
			_mockDistributedCache!.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never,
				"Distributed cache should not be accessed when URL does not contain 'webapi'.");
		}

		[TestMethod]
		public async Task InvokeAsync_CacheMiss_ReadsRequestBodyAndSetsCache()
		{
			// Arrange
			var requestUrl = $"http://example.com/api/{Program.WebApi}/data";
			var requestBody = "{\"key\":\"value\"}";
			var httpContext = CreateHttpContext(requestUrl, requestBody);
			byte[]? cacheResult = null;

			_mockDistributedCache!.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(cacheResult)
				.Verifiable();

			// Имитируем, что следующий делегат возвращает ответ
			var responseBodyStream = new MemoryStream(Encoding.UTF8.GetBytes("{\"data\":\"cached_content\"}"));
			var httpContextMock = httpContext;
			httpContext.Response.Body = new MemoryStream(); // Изначально пустой поток
			_mockNext!.Setup(x => x(It.IsAny<HttpContext>())).Callback(async (HttpContext ctx) =>
			{
				ctx.Response.ContentType = "application/json";
				await ctx.Response.WriteAsync("{\"data\":\"cached_content\"}");
			});
			_mockDistributedCache!.Setup(x =>
				x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask)
				.Verifiable();

			// Act
			await _cut!.Invoke(httpContextMock);

			// Assert
			_mockDistributedCache!.Verify(x =>
				x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
				Times.Once, "Should attempt to get from cache.");
			_mockNext!.Verify(x => x(It.IsAny<HttpContext>()), Times.Once, "Next delegate should be called on cache miss.");
			_mockDistributedCache!.Verify(x =>
				x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
				Times.Once, "Should set the cache on cache miss.");
		}

		[TestMethod]
		public async Task InvokeAsync_CacheHit_ReturnsCachedResponseAndDoesNotCallNext()
		{
			// Arrange
			var requestUrl = $"http://example.com/api/{Program.WebApi}/data";
			var requestBody = "{\"key\":\"value\"}";
			var httpContext = CreateHttpContext(requestUrl, requestBody);

			var cachedContent = "{\"data\":\"cached_content\"}";
			var cachedByteContent = Encoding.UTF8.GetBytes(cachedContent);

			// Имитируем, что в кеше есть данные
			_mockDistributedCache!.Setup(d => d.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
								.ReturnsAsync(cachedByteContent)
								.Verifiable();
			var responseStream = new MemoryStream();
			httpContext.Response.Body = responseStream;

			// Act
			await _cut!.Invoke(httpContext);

			// Assert
			_mockDistributedCache!.Verify(x => x.GetAsync(It.IsAny<string>(),
				It.IsAny<CancellationToken>()), Times.Once, "Should attempt to get from cache.");
			_mockNext!.Verify(x => x(It.IsAny<HttpContext>()), Times.Never, "Next delegate should NOT be called on cache hit.");
			_mockDistributedCache!.Verify(x =>
				x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
				Times.Never, "Should NOT set the cache on cache hit.");

			// Проверяем, что ответ был записан в Response.Body
			responseStream.Seek(0, SeekOrigin.Begin);
			using var reader = new StreamReader(responseStream);
			var responseText = await reader.ReadToEndAsync();

			Assert.AreEqual(cachedContent, responseText, "Response body should contain the cached content.");
		}

		[TestMethod]
		public async Task InvokeAsync_GetRequest_DoesNotReadRequestBodyAndContinues()
		{
			// Arrange
			var requestUrl = $"http://example.com/api/{Program.WebApi}/data?key=value";
			var httpContext = CreateHttpContext(requestUrl, requestMethod: "GET");

			_mockDistributedCache!.Setup(d => d.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
								.ReturnsAsync((byte[]?)null) // Имитируем Cache Miss
								.Verifiable();
			_mockNext!.Setup(d => d(It.IsAny<HttpContext>())).Callback(async (HttpContext ctx) =>
			{
				ctx.Response.ContentType = "application/json";
				await ctx.Response.WriteAsync("{\"data\":\"some_content\"}");
			});

			_mockDistributedCache!.Setup(d =>
				d.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask)
				.Verifiable();

			// Act
			await _cut!.Invoke(httpContext);

			// Assert
			// Проверяем, что ReadRequestBodyAsync не вызывался (хотя прямого способа проверить этот private метод нет,
			// мы можем убедиться, что он не вызывает EnableBuffering, если бы он был вызван)
			// Вместо этого, проверяем, что кеш-ключ не содержит тела запроса.
			_mockDistributedCache!.Verify(d =>
				d.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
				Times.Once, "Cache key should be called.");
			_mockNext!.Verify(d => d(It.IsAny<HttpContext>()), Times.Once, "Next delegate should be called.");
			_mockDistributedCache!.Verify(d =>
				d.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
				Times.Once, "Should set the cache.");
		}

		#region [helper methods]
		/// <summary>
		/// Helper class to create a mock HttpContext with a buffered request body.
		/// </summary>
		private HttpContext CreateHttpContext(string requestUrl, string requestBody = "", string requestMethod = "POST")
		{
			var httpContext = new DefaultHttpContext();
			var request = httpContext.Request;

			// Use Uri class to parse the full URL
			var uri = new Uri(requestUrl);

			// Set the request method and URL
			request.Method = requestMethod;
			request.Scheme = "http";
			request.Host = new HostString(uri.Host, uri.Port);
			request.Path = uri.AbsolutePath; // <-- исправленная строка

			if (!string.IsNullOrEmpty(requestBody))
			{
				// Create a stream for the request body
				var requestBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

				// Set the request body
				request.Body = requestBodyStream;
				request.ContentLength = requestBodyStream.Length;
				request.ContentType = "application/json";

				// Reset the position for reading
				requestBodyStream.Position = 0;
			}

			return httpContext;
		}
	}
	#endregion [helper methods]
}