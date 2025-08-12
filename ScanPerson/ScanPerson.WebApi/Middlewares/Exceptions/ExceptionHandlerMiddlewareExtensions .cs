namespace ScanPerson.WebApi.Middlewares.Exceptions
{
	public static class ExceptionHandlerMiddlewareExtensions
	{
		public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<ExceptionHandlerMiddleware>();
		}
	}
}
