using ScanPerson.Common.Resources;

namespace ScanPerson.Common.Helpers
{
	public class EnviromentHelper
	{
		public static string GetViriableByName(string variableName)
		{
			return Environment.GetEnvironmentVariable(variableName)
							?? throw new InvalidOperationException(string.Format(Messages.EnvironmentNotFound, variableName));
		}
	}
}
