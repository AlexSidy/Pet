using ScanPerson.WebApi.Middlewares;

namespace ScanPerson.WebApi.Extensions
{
	/// <summary>
	/// Extension methods for adding the ExceptionHandlerMiddleware to the application's request pipeline.
	/// </summary>
	public static class ScanPersonMiddlewareExtensions
	{
		/// <summary>
		/// Adds the ExceptionHandlerMiddleware to the specified IApplicationBuilder.
		/// </summary>
		/// <param name="app">The IApplicationBuilder to which the middleware is added.</param>
		public static void UseScanPersonMiddlewares(this IApplicationBuilder app)
		{
			app.UseMiddleware<ExceptionHandlerMiddleware>();
			app.UseMiddleware<RedisCacheMiddleware>();
		}
	}
}