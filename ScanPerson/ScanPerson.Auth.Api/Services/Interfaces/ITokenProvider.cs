namespace ScanPerson.Auth.Api.Services.Interfaces
{
	/// <summary>
	/// Provider for token generate.
	/// </summary>
	public interface ITokenProvider
	{
		/// <summary>
		/// Generate token with claims.
		/// </summary>
		/// <param name="user">User.</param>
		/// <returns>Token value.</returns>
		Task<string> GenerateTokenAsync(User user);
	}
}
