using Microsoft.AspNetCore.Authorization;

using ScanPerson.Models.Options;

namespace ScanPerson.WebApi.Filters;

/// <summary>
/// Handler to skip authorization for truted hosts (internaal docker network).
/// </summary>
/// <param name="logger">Logger.</param>
/// <param name="options">Options with trusted hosts.</param>
public class OrRequirementsHandler(
	ILogger<OrRequirementsHandler> logger,
	HostWhiteListOptions options) : AuthorizationHandler<IAuthorizationRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
	{
		return Task.CompletedTask;
	}

	/// <summary>
	/// Processing multiple requirements and return success if at least one requirement is success.
	/// </summary>
	/// <param name="context">Contains authorization information used by <see cref="IAuthorizationHandler"/>.</param>
	public override Task HandleAsync(AuthorizationHandlerContext context)
	{
		if (context.PendingRequirements.Count() < context.Requirements.Count())
		{
			foreach(var requirement in context.Requirements) {
				context.Succeed(requirement);
			}
		}
		else
		{
			context.Fail();
		}

			return Task.CompletedTask;
	}
}