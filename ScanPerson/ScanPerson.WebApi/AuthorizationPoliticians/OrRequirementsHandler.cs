using Microsoft.AspNetCore.Authorization;

namespace ScanPerson.WebApi.AuthorizationPoliticians;

/// <summary>
/// Handler to skip authorization for truted hosts (internaal docker network).
/// </summary>
/// <param name="logger">Logger.</param>
public class OrRequirementsHandler(
	ILogger<OrRequirementsHandler> logger) : AuthorizationHandler<IAuthorizationRequirement>
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
			logger.LogInformation("One of the requirements is success");
			foreach (var requirement in context.Requirements) {
				context.Succeed(requirement);
			}
		}
		else
		{
			logger.LogInformation("All requirements are not success");
			context.Fail();
		}

			return Task.CompletedTask;
	}
}