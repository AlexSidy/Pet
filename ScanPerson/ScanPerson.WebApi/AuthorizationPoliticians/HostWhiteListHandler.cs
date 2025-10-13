using Microsoft.AspNetCore.Authorization;

using ScanPerson.BusinessLogic.Managers;

namespace ScanPerson.WebApi.AuthorizationPoliticians;

/// <summary>
/// Handler to skip authorization for truted hosts (internaal docker network).
/// </summary>
/// <param name="logger">Logger.</param>
/// <param name="options">Options with trusted hosts.</param>
public class HostWhiteListHandler(ILogger<HostWhiteListHandler> logger, HttpContextManager httpContextManager)
	: AuthorizationHandler<HostWhiteListRequirement>
{
	/// <summary>
	/// Handler to skip authorization for truted hosts (internaal docker network).
	/// </summary>
	/// <param name="context">Contains authorization information used by <see cref="IAuthorizationHandler"/>.</param>
	/// <param name="requirement">Requirement for trusted hosts (internaal docker network).</param>
	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HostWhiteListRequirement requirement)
	{
		var isInternalRequest = await httpContextManager.IsInternalRequestAsync(context.Resource as HttpContext);
		if (isInternalRequest)
		{
			logger.LogInformation("Authorization skipped.");

			context.Succeed(requirement);
		}
	}
}