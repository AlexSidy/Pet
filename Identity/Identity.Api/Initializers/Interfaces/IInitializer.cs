namespace Identity.Api.Initializers.Interfaces;

/// <summary>
/// Interface for initial data seeding in a service.
/// </summary>
internal interface IInitializer
{
	/// <summary>
	/// Method for requesting data, validating it, and performing further initialization if necessary.
	/// </summary>
	/// <returns>A task that represents the asynchronous initialization operation.</returns>
	Task SeedAsync();
}