using ScanPerson.Models.Attributes;

namespace ScanPerson.Models.Options
{
	/// <summary>
	/// Settings for used the cache.
	/// </summary>
	public class CacheOptions
	{
		/// <summary>
		/// Flag to enable or disable the cache.
		/// </summary>
		[EnvironmentVariable("IS_CACHE_ENABLE")]
		public bool IsEnable { get; set; }

		/// <summary>
		/// Absolute expiration relative to now.
		/// </summary>
		[EnvironmentVariable("CACHE_EXPIRATION")]
		public int CacheExpiration { get; set; }
	}
}