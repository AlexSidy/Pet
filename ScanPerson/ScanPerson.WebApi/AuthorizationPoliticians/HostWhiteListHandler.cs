using System.Net;

using Microsoft.AspNetCore.Authorization;

using ScanPerson.Models.Options;

namespace ScanPerson.WebApi.AuthorizationPoliticians;

/// <summary>
/// Handler to skip authorization for truted hosts (internaal docker network).
/// </summary>
/// <param name="logger">Logger.</param>
/// <param name="options">Options with trusted hosts.</param>
public class HostWhiteListHandler(
	ILogger<HostWhiteListHandler> logger,
	HostWhiteListOptions options) : AuthorizationHandler<HostWhiteListRequirement>
{
	/// <summary>
	/// Handler to skip authorization for truted hosts (internaal docker network).
	/// </summary>
	/// <param name="context">Contains authorization information used by <see cref="IAuthorizationHandler"/>.</param>
	/// <param name="requirement">Requirement for trusted hosts (internaal docker network).</param>
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HostWhiteListRequirement requirement)
	{
		var httpContext = context.Resource as HttpContext;
		if (httpContext == null)
		{
			return Task.CompletedTask;
		}

		var remoteIp = httpContext.Connection.RemoteIpAddress?.MapToIPv4();
		var allowedIps = options.WhiteHosts.SelectMany(x => Dns.GetHostEntry(x).AddressList);
		if (remoteIp != null && allowedIps.Contains(remoteIp))
		{
			logger.LogInformation("Authorization skipped for trusted IP: {RemoteIp}", new { RemoteIp = remoteIp });

			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}