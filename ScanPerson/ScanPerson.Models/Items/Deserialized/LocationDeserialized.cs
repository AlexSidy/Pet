using System.Text.Json.Serialization;

namespace ScanPerson.Models.Items
{
	/// <summary>
	/// Object for receiving data from the service https://htmlweb.ru/geo/telcod_api_example.php
	/// </summary>
	public class LocationDeserialized
	{
		/// <summary>
		/// Country of registration of the phone number.
		/// </summary>
		public Country Country { get; set; }

		/// <summary>
		/// Presumably the current position of the phone.
		/// </summary>
		public Region Region { get; set; }

		/// <summary>
		/// District of registration of telephone number.
		/// </summary>
		public string Okrug { get; set; }

		/// <summary>
		/// City of registration of the phone number.
		/// </summary>
		public Capital Capital { get; set; }

		/// <summary>
		/// Communication service operator.
		/// </summary>
		[JsonPropertyName("0")]
		public Operator Operator { get; set; }
	}

	public class Country
	{
		public string Name { get; set; }
	}

	public class Region
	{
		public string Name { get; set; }
	}

	public class Capital
	{
		public string Name { get; set; }
	}

	public class Operator
	{
		/// <summary>
		/// City of service operator.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Operator name.
		/// </summary>
		[JsonPropertyName("oper_brand")]
		public string OperBrand { get; set; }
	}
}