namespace ScanPerson.Models.Options
{
	/// <summary>
	/// Settings for used the host options.
	/// </summary>
	public class ServiceHostOptions
	{
		/// <summary>
		/// Name of host.
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Port of host.
		/// </summary>
		public int? Port { get; set; }

		/// <summary>
		/// Version of api.
		/// </summary>
		public string ApiVersion { get; set; }

		/// <summary>
		/// Name of controller.
		/// </summary>
		public string ControllerName { get; set; }
	}
}