using System.Data;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace ScanPerson.Common.Extensions
{
	public static class ServiceExtensions
	{
		public static void AddAllImplementations<T>(this IServiceCollection services)
		{
			// Находим все типы, которые реализуют интерфейс T
			var implementations = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

			foreach (var implementation in implementations)
			{
				// Регистрируем каждую реализацию
				services.AddTransient(typeof(T), implementation);
			}
		}
	}
}
