namespace ScanPerson.Models.Items
{
	/// <summary>
	/// Локационные данные персоны.
	/// </summary>
	public class LocationItem
	{
		/// <summary>
		/// Страна регистрации номера телефона.
		/// </summary>
		public string CountryName { get; set; }

		/// <summary>
		/// Предположительно, текущее положение телефона.
		/// </summary>
		public string CurrentRegion { get; set; }

		/// <summary>
		/// Округ регистрации номера телефона.
		/// </summary>
		public string RegistrationOkrug { get; set; }

		/// <summary>
		/// Город  регистрации номера телефона.
		/// </summary>
		public string RegistrationCapital { get; set; }

		/// <summary>
		/// Оператор обслуживания связи.
		/// </summary>
		public string OperatorName { get; set; }

		/// <summary>
		/// Город оператора обслуживания.
		/// </summary>
		public string OperatorCity { get; set; }
	}
}