using System.Data;
using System.Reflection;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace ScanPerson.Common.Extensions
{
	public static class ServiceExtensions
	{
		/// <summary>
		/// Добавление всех реализаций интерфейса.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="services"></param>
		public static void AddAllImplementations<T>(this IServiceCollection services)
		{
			var implementations = Assembly
				.GetCallingAssembly()!
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
