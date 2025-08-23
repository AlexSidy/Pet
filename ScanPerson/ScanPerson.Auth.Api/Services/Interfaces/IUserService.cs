using ScanPerson.Models.Requests.Auth;
using ScanPerson.Models.Responses;

namespace ScanPerson.Auth.Api.Services.Interfaces
{
	/// <summary>
	/// Service for user.
	/// </summary>
	public interface IUserService
	{
		/// <summary>
		/// User registration.
		/// </summary>
		/// <param name="request">Register informaation.</param>
		/// <returns>Operation result with erorr information.</returns>
		Task<ScanPersonResponseBase> RegisterAsync(RegisterRequest request);

		/// <summary>
		/// User Login.
		/// </summary>
		/// <param name="request">Login informaation.</param>
		/// <returns>Operation result with erorr information.</returns>
		Task<ScanPersonResponseBase> LoginAsync(LoginRequest request);
	}
}
