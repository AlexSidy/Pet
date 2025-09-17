using System.Net;

using Newtonsoft.Json;

using ScanPerson.Common.Resources;

namespace ScanPerson.WebApi.Middlewares
{
	/// <summary>
	/// Middleware to handle exceptions globally.
	/// </summary>
	public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
	{
		/// <summary>
		/// Invokes the middleware.
		/// </summary>
		/// <param name="context">The HTTP context.</param>
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await next.Invoke(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, Messages.UnhandledException, ex.Message);
				await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Handles the exception and writes a JSON response.
		/// </summary>
		/// <param name="context">The HTTP context.</param>
		/// <param name="exception">The exception that occurred.</param>
		private static async Task HandleExceptionMessageAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			int statusCode = (int)HttpStatusCode.InternalServerError;
			context.Response.StatusCode = statusCode;
			var result = JsonConvert.SerializeObject(new
			{
				StatusCode = statusCode,
				ErrorMessage = exception.Message
			});
			await context.Response.WriteAsync(result);
		}
	}
}
