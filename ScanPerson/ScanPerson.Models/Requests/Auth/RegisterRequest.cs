using System.ComponentModel.DataAnnotations;

namespace ScanPerson.Models.Requests.Auth
{
	public class RegisterRequest
	{
		/// <summary>
		/// Password.
		/// </summary>
		[Required]
		public string Password { get; set; }

		/// <summary>
		/// User`s email.
		/// </summary>
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
