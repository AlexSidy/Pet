namespace ScanPerson.Models.Contracts
{
	/// <summary>
	/// Ключи доступа к различныи сервисам используемым приложением.
	/// </summary>
	public class ScanPersonSecrets
	{
		/// <summary>
		/// Ключ доступа к сервису https://htmlweb.ru/geo/telcod_api_example.php.
		/// </summary>
		public string HtmlWebRuApiKey { get; set; }
	}
}
