using System.Data;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace ScanPerson.Common.Extensions
{
	public static class ServiceExtensions
	{
		/// <summary>
		/// Adding all types that implement the interface.
		/// </summary>
		/// <typeparam name="T">Type for registration.</typeparam>
		/// <param name="services">Aplication services.</param>
		public static void AddAllImplementations<T>(this IServiceCollection services)
		{
			var implementations = Assembly
				.GetCallingAssembly()!
				.GetTypes()
				.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

			foreach (var implementation in implementations)
			{
				services.AddTransient(typeof(T), implementation);
			}
		}
	}
}
