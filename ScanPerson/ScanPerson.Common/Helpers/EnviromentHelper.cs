using ScanPerson.Common.Resources;

namespace ScanPerson.Common.Helpers
{
	/// <summary>
	/// Helper class for processing and interaction environment variables.
	/// </summary>
	public static class EnviromentHelper
	{
		public static string GetViriableByName(string variableName)
		{
			return Environment.GetEnvironmentVariable(variableName)
							?? throw new InvalidOperationException(string.Format(Messages.EnvironmentNotFound, variableName));
		}
	}
}
