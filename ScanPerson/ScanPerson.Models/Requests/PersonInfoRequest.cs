using System.ComponentModel.DataAnnotations;

namespace ScanPerson.Models.Requests
{
	/// <summary>
	/// Input data for retrieving information about a person.
	/// </summary>
	public class PersonInfoRequest
	{
		/// <summary>
		/// The phone number.
		/// </summary>
		[Required]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// Name of the person.
		/// </summary>	
		public string FirstName { get; set; }

		/// <summary>
		/// Second name of the person.
		/// </summary>
		public string SecondName { get; set; }

		/// <summary>
		/// Patronymic of the person.
		/// </summary>
		public string Patronymic { get; set; }
	}
}