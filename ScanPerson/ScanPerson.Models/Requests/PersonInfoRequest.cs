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
		public string PhoneNumber { get; set; }
	}
}