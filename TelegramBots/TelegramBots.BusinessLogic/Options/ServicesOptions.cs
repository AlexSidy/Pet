using ScanPerson.Models.Options;

namespace TelegramBots.BusinessLogic.Options
{
	/// <summary>
	/// Settings for the services used.
	/// </summary>
	public class ServicesOptions
	{
		public ServicesOptions()
		{
			ScanPersonWebApiServiceOptions = new ScanPersonWebApiServiceOptions();
		}

		/// <summary>
		/// The name of the section in appsettings.json.
		/// </summary>
		public const string AppSettingsSection = "ServiceOptions";


		public ScanPersonWebApiServiceOptions ScanPersonWebApiServiceOptions { get; set; }
	}


	/// <summary>
	/// Settings for the scan person web api service.
	/// </summary>
	public class ScanPersonWebApiServiceOptions
	{
		/// <summary>
		/// The host options for the service.
		/// </summary>
		public ServiceHostOptions? ServiceHostOptions { get; set; }
	}
}