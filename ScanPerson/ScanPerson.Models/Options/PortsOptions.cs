using ScanPerson.Models.Attributes;

namespace ScanPerson.Models.Options
{
	/// <summary>
	/// Settings for used the ports.
	/// </summary>
	public class PortsOptions
	{
		/// <summary>
		/// Port for http connection.
		/// </summary>
		[EnvironmentVariable("ASPNETCORE_HTTP_PORTS")]
		public int HttpPort { get; set; }

		/// <summary>
		/// Port for https connection.
		/// </summary>
		[EnvironmentVariable("ASPNETCORE_HTTPS_PORTS")]
		public int HttpsPort { get; set; }

		/// <summary>
		/// Port for grpc connection.
		/// </summary>
		[EnvironmentVariable("GRPC_PORTS")]
		public int GrpcPort { get; set; }
	}
}