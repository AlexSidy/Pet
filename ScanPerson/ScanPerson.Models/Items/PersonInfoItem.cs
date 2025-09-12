namespace ScanPerson.Models.Items
{
	/// <summary>
	/// Information about a person.
	/// </summary>
	public class PersonInfoItem
	{
		/// <summary>
		/// Unique identifier.
		/// </summary>
		public long? Id { get; set; }

		/// <summary>
		/// Name of the person.
		/// </summary>	
		public string Name { get; set; }

		/// <summary>
		/// Mail of the person.
		/// </summary>
		public string Mail { get; set; }

		/// <summary>
		/// Location of the person.
		/// </summary>
		public LocationItem Location { get; set; }
	}
}
