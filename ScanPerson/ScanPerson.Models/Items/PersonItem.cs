namespace ScanPerson.Models.Items
{
	public class PersonItem
	{
		public PersonItem(long id, string name, string mail)
		{
			Id = id;
			Name = name;
			Mail = mail;
		}

		public long Id { get; set; }

		public string Name { get; set; }

		public string Mail { get; set; }
	}
}
