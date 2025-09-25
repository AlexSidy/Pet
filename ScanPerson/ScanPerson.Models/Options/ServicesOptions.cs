namespace ScanPerson.Models.Options
{
	/// <summary>
	/// Settings for the services used.
	/// </summary>
	public class ServicesOptions
	{
		public ServicesOptions()
		{
			GeoServiceOptions = new GeoServiceOptions();
			BrowserBotServiceOptions = new BrowserBotServiceOptions();
			UnUsingServices = new string[] { };
		}

		/// <summary>
		/// The name of the section in appsettings.json.
		/// </summary>
		public const string AppSettingsSection = "ServiceOptions";

		/// <summary>
		/// A list of disabled services.
		/// </summary>
		public string[] UnUsingServices { get; set; }

		public GeoServiceOptions GeoServiceOptions { get; set; }

		public BrowserBotServiceOptions BrowserBotServiceOptions { get; set; }
	}

	/// <summary>
	/// Geolocation service settings.
	/// </summary>
	public class GeoServiceOptions
	{
		/// <summary>
		/// The base API URL for the service.
		/// </summary>
		public string BaseUrl { get; set; }
	}

	/// <summary>
	/// Settings for the browser bot service.
	/// </summary>
	public class BrowserBotServiceOptions
	{
		/// <summary>
		/// The host options for the service.
		/// </summary>
		public ServiceHostOptions ServiceHostOptions { get; set; }
	}
}