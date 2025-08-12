using System.Net;

using Newtonsoft.Json;

namespace ScanPerson.WebApi.Middlewares.Exceptions
{
	public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
	{
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await next.Invoke(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message);
				await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
			}
		}

		private static Task HandleExceptionMessageAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			int statusCode = (int)HttpStatusCode.InternalServerError;
			context.Response.StatusCode = statusCode;
			var result = JsonConvert.SerializeObject(new
			{
				StatusCode = statusCode,
				ErrorMessage = exception.Message
			});
			return context.Response.WriteAsync(result);
		}
	}
}
