namespace ScanPerson.Models.Requests
{
	/// <summary>
	/// входные данные для получения информации о человеке.
	/// </summary>
	public class PersonInfoRequest
	{
		/// <summary>
		/// Номер телефона.
		/// </summary>
		public string PhoneNumber { get; set; }
	}
}
