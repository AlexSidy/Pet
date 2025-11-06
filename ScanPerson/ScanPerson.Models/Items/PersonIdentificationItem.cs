using System.Text.Json.Serialization;

namespace ScanPerson.Models.Items
{
	/// <summary>
	/// Information about a person.
	/// </summary>
	public class PersonIdentificationItem
	{
		[JsonPropertyName("FirstName")]
		public string FirstName { get; set; } = string.Empty;

		[JsonPropertyName("LastName")]
		public string SecondName { get; set; } = string.Empty;

		[JsonPropertyName("Patronymic")]
		public string Patronymic { get; set; } = string.Empty;

		public override string ToString()
		{
			return $"Имя: {FirstName}," +
					$"Фамилия: {SecondName}," +
					$"Отчество: {Patronymic}";
		}

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(SecondName);
			}
		}
	}
}
