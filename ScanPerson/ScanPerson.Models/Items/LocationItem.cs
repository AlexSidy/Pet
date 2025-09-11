namespace ScanPerson.Models.Items
{
	/// <summary>
	/// Location data of a person.
	/// </summary>
	public class LocationItem
	{
		/// <summary>
		/// The country where the phone number is registered.
		/// </summary>
		public string CountryName { get; set; }

		/// <summary>
		/// The presumed current location of the phone.
		/// </summary>
		public string CurrentRegion { get; set; }

		/// <summary>
		/// The district where the phone number is registered.
		/// </summary>
		public string RegistrationOkrug { get; set; }

		/// <summary>
		/// The city where the phone number is registered.
		/// </summary>
		public string RegistrationCapital { get; set; }

		/// <summary>
		/// The telecommunications service provider.
		/// </summary>
		public string OperatorName { get; set; }

		/// <summary>
		/// The city of the service provider.
		/// </summary>
		public string OperatorCity { get; set; }
	}
}