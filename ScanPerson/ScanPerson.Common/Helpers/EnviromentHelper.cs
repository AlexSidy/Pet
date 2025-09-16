using System.Reflection;

using ScanPerson.Common.Resources;
using ScanPerson.Models.Attributes;

namespace ScanPerson.Common.Helpers
{
	/// <summary>
	/// Helper class for processing and interaction environment variables.
	/// </summary>
	public static class EnviromentHelper
	{
		public static string GetVariableByName(string variableName)
		{
			return Environment.GetEnvironmentVariable(variableName)
							?? throw new InvalidOperationException(string.Format(Messages.EnvironmentNotFound, variableName));
		}

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
