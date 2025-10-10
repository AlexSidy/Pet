using Microsoft.AspNetCore.Authorization;

namespace ScanPerson.WebApi.AuthorizationPoliticians;

/// <summary>
/// Requirement for trusted hosts (internaal docker network).
/// </summary>
public class HostWhiteListRequirement : IAuthorizationRequirement { }
