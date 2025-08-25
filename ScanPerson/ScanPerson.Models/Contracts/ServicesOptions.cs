namespace ScanPerson.Models.Contracts
{
	/// <summary>
	/// Настройки используемых сервисов.
	/// </summary>
	public class ServicesOptions
	{
		public ServicesOptions()
		{
			GeoServiceOptions = new GeoServiceOptions();
		}

		/// <summary>
		/// Название секции в appsettings.json.
		/// </summary>
		public const string AppSettingsSection = "ServicesOptions";

		/// <summary>
		/// Список отключенных сервисов.
		/// </summary>
		public string[] UnUsingServices { get; set; }

		public GeoServiceOptions GeoServiceOptions { get; set; }
	}

	/// <summary>
	/// Настройки сервиса геолокации.
	/// </summary>
	public class GeoServiceOptions
	{
		/// <summary>
		/// Базовый апи сервиса.
		/// </summary>
		public string BaseUrl { get; set; }
	}
}