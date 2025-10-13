namespace ScanPerson.Models.Options
{
	/// <summary>
	/// Settings for host from white list.
	/// </summary>
	public class HostWhiteListOptions
	{
		public HostWhiteListOptions(string[] whiteHosts)
		{
			WhiteHosts = whiteHosts;
		}

		/// <summary>
		/// List of white hosts.
		/// </summary>
		public string[] WhiteHosts { get; }
	}
}