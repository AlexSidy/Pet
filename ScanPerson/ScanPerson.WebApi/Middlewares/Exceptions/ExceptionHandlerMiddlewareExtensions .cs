namespace ScanPerson.WebApi.Middlewares.Exceptions
{
	/// <summary>
	/// Extension methods for adding the ExceptionHandlerMiddleware to the application's request pipeline.
	/// </summary>
	public static class ExceptionHandlerMiddlewareExtensions
	{
		/// <summary>
		/// Adds the ExceptionHandlerMiddleware to the specified IApplicationBuilder.
		/// </summary>
		/// <param name="app">The IApplicationBuilder to which the middleware is added.</param>
		public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<ExceptionHandlerMiddleware>();
		}
	}
}