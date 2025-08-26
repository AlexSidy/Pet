using System.Text.Json.Serialization;

namespace ScanPerson.Models.Items
{
	/// <summary>
	/// Объект для получения данных с сервиса https://htmlweb.ru/geo/telcod_api_example.php
	/// </summary>
	public class LocationDeserialized
	{
		/// <summary>
		/// Страна регистрации номера телефона.
		/// </summary>
		public Country Country { get; set; }

		/// <summary>
		/// Предположительно, текущее положение телефона.
		/// </summary>
		public Region Region { get; set; }

		/// <summary>
		/// Округ регистрации номера телефона.
		/// </summary>
		public string Okrug { get; set; }

		/// <summary>
		/// Город  регистрации номера телефона.
		/// </summary>
		public Capital Capital { get; set; }

		/// <summary>
		/// Оператор обслуживания связи.
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
		/// Город оператора обслуживания.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Название оператора.
		/// </summary>
		[JsonPropertyName("oper_brand")]
		public string OperBrand { get; set; }
	}
}