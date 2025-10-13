using System.Net;
using System.Net.Sockets;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using ScanPerson.Models.Options;

namespace ScanPerson.BusinessLogic.Managers
{
	/// <summary>
	/// Manager for http context.
	/// </summary>
	/// <param name="logger">Logging service.</param>
	/// <param name="options"></param>
	public class HttpContextManager(ILogger<HttpContextManager> logger, HostWhiteListOptions options)
	{
		/// <summary>
		/// Check is internal request.
		/// </summary>
		/// <param name="httpContext">Http context.</param>
		/// <returns>True if internal request.</returns>
		public async Task<bool> IsInternalRequestAsync(HttpContext? httpContext)
		{
			logger.LogInformation("Check Is internal for request: {Path}",
				new { Path = JsonSerializer.Serialize(httpContext?.Request.Path) });
			if (httpContext == null)
			{
				return false;
			}

			var remoteIp = httpContext.Connection.RemoteIpAddress?.MapToIPv4();
			if (remoteIp == null)
			{
				return false;
			}
			var resolvedIpArrays = await Task.WhenAll(options.WhiteHosts.Select(host => GetIpAddressesSafeAsync(host)));
			var allowedIps = resolvedIpArrays.SelectMany(ipArray => ipArray);
			if (allowedIps.Contains(remoteIp))
			{
				logger.LogInformation("Request is internal for trusted IP: {RemoteIp}", new { RemoteIp = remoteIp });
				return true;
			}

			return false;
		}

		/// <summary>
		/// Resolve host name to ip address.
		/// </summary>
		/// <param name="hostName">Host name.</param>
		/// <returns></returns>
		private async Task<IPAddress[]> GetIpAddressesSafeAsync(string hostName)
		{
			if (string.IsNullOrWhiteSpace(hostName))
			{
				return [];
			}

			try
			{
				var entry = await Dns.GetHostEntryAsync(hostName);

				return entry.AddressList;
			}
			catch (SocketException ex)
			{
				logger.LogError(ex, "DNS resolution error for {Error}", new { Error = ex.Message });

				return [];
			}
		}
	}
}