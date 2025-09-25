using System.Reflection;

using Microsoft.Extensions.Configuration;

using ScanPerson.Common.Resources;
using ScanPerson.Models.Attributes;
using ScanPerson.Models.Options;
using ScanPerson.Models.Options.Auth;

namespace ScanPerson.Common.Helpers
{
	/// <summary>
	/// Helper class for processing and interaction environment variables.
	/// </summary>
	public static class EnviromentHelper
	{
		/// <summary>
		/// Get environment variable by name.
		/// </summary>
		/// <param name="variableName">Variable name.</param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Throws when variable not found.</exception>
		public static string GetVariableByName(string variableName)
		{
			return Environment.GetEnvironmentVariable(variableName)
							?? throw new InvalidOperationException(string.Format(Messages.EnvironmentNotFound, variableName));
		}

		/// <summary>
		/// Get host options by section name.
		/// </summary>
		/// <param name="serviceName">Service name.</param>
		/// <param name="configuration">App configuration.</param>
		/// <returns>Host options.</returns>
		/// <exception cref="InvalidOperationException">Throws when section not found.</exception>
		public static ServiceHostOptions GetHostOptionsBySectionByName(string serviceName, IConfiguration configuration)
		{
			var hostOptions = configuration.GetSection(serviceName).Get<ServiceHostOptions>()
				?? throw new InvalidOperationException(string.Format(Messages.SectionNotFound, serviceName));

			return hostOptions;
		}


		/// <summary>
		/// Get filled model from environment variables.
		/// </summary>
		/// <typeparam name="T">Model type.</typeparam>
		/// <returns>Model with filled properties.</returns>
		/// <exception cref="InvalidOperationException">Throws when failed to convert environment variable.</exception>
		public static T GetFilledFromEnvironment<T>() where T : class, new()
		{
			var properties = typeof(T).GetProperties()
				.Where(p => p.IsDefined(typeof(EnvironmentVariableAttribute), false));
			var result = new T();

			foreach (var prop in properties)
			{
				var attribute = prop.GetCustomAttribute<EnvironmentVariableAttribute>();
				if (attribute == null)
				{
					continue;
				}

				var envValue = GetVariableByName(attribute.Name);

				try
				{
					var convertedValue = Convert.ChangeType(envValue, prop.PropertyType);
					prop.SetValue(result, convertedValue);
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"Could not convert environment variable '{attribute.Name}' to type '{prop.PropertyType.Name}'.", ex);
				}

			}

			return result;
		}
	}
}
