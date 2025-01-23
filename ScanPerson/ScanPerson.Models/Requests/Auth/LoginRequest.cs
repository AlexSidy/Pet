using System.ComponentModel.DataAnnotations;

namespace ScanPerson.Models.Requests.Auth
{
	/// <summary>
	/// Logins contract.
	/// </summary>
	public class LoginRequest
	{
		/// <summary>
		/// Password.
		/// </summary>
		[Required]
		public string Password { get; set; }

		/// <summary>
		/// Users email.
		/// </summary>
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
