using ScanPerson.Models.Attributes;

namespace ScanPerson.Models.Options
{
	/// <summary>
	/// Access keys to various services used by the application.
	/// </summary>
	public class ScanPersonSecrets
	{
		/// <summary>
		/// Access key for the service https://htmlweb.ru/geo/telcod_api_example.php.
		/// </summary>
		[EnvironmentVariable("HTMLWEBRU_API_KEY")]
		public string HtmlWebRuApiKey { get; set; }
	}
}