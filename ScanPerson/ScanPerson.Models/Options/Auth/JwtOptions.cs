namespace ScanPerson.Models.Options.Auth
{
	/// <summary>
	/// Description of the settings for the authentication token.
	/// </summary>
	public class JwtOptions
	{
		/// <summary>
		/// The name of the section in appsettings.json
		/// </summary>
		public const string AppSettingsSection = "JwtOptions";

		/// <summary>
		/// The token's issuer.
		/// </summary>
		public string Issuer { get; set; }

		/// <summary>
		/// The token's audience.
		/// </summary>
		public string Audience { get; set; }

		/// <summary>
		/// The key for encryption.
		/// </summary>
		public string SecretKey { get; set; }

		/// <summary>
		/// The token's expiration in hours.
		/// </summary>
		public int ExpireHours { get; set; }
	}
}