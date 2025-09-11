namespace ScanPerson.Models.Items
{
	public class PersonInfoItem
	{
		public long? Id { get; set; }

		public string Name { get; set; }

		public string Mail { get; set; }

		public LocationItem Location { get; set; }
	}
}
