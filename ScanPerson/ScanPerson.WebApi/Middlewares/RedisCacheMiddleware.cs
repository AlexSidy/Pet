using System.Text;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;

using ScanPerson.Models.Options;

namespace ScanPerson.WebApi.Middlewares
{
	/// <summary>
	/// Middleware to handle cache globally.
	/// </summary>
	public class RedisCacheMiddleware(
		ILogger<RedisCacheMiddleware> logger,
		IDistributedCache cache,
		CacheOptions cacheOptions,
		RequestDelegate next)
	{
		/// <summary>
		/// Invokes the middleware.
		/// </summary>
		/// <param name="context">The HTTP context.</param>
		public async Task Invoke(HttpContext context)
		{
			var url = context.Request.GetDisplayUrl();
			if (!cacheOptions.IsEnable || !url.Contains(Program.WebApi))
			{
				await next(context);
				return;
			}

			// Form a unique cache key based on the URL and request body.
			var requestBody = await ReadRequestBodyAsync(context.Request);
			var cacheKey = $"{url}-{requestBody}";

			logger.LogInformation($"Checking cache for key: {cacheKey}");

			var cachedResponse = await cache.GetStringAsync(cacheKey);

			if (!string.IsNullOrEmpty(cachedResponse))
			{
				logger.LogInformation($"Cache hit for key: {cacheKey}");
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync(cachedResponse);
				return;
			}
			else
			{
				logger.LogInformation($"Cache miss for key: {cacheKey}");
				// If no cached response is found, capture the response body.
				var originalBodyStream = context.Response.Body;
				await using var responseBody = new MemoryStream();
				context.Response.Body = responseBody;

				await next(context);

				// Copy the response to not interfere with the original stream.
				responseBody.Seek(0, SeekOrigin.Begin);
				using var reader = new StreamReader(responseBody);
				var responseText = await reader.ReadToEndAsync();

				// Reset the response stream to the beginning.
				responseBody.Seek(0, SeekOrigin.Begin);
				await responseBody.CopyToAsync(originalBodyStream);
				context.Response.Body = originalBodyStream;

				var options = new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheOptions.CacheExpiration)
				};
				await cache.SetStringAsync(cacheKey, responseText, options);
			}
		}

		/// <summary>
		/// Getting request body as text.
		/// </summary>
		/// <param name="request">Object of request.</param>
		/// <returns>Request body as a string.</returns>
		private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
		{
			if (request.Method == "GET")
			{
				return string.Empty;
			}

			// Allows you to read the request body multiple times
			request.EnableBuffering();

			// Read the body into a stream.
			using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
			var bodyAsText = await reader.ReadToEndAsync();

			// Rewind the stream to the beginning so that subsequent middleware can read it.
			request.Body.Position = 0;

			return bodyAsText;
		}
	}
}
