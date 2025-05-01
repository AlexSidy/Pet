namespace ScanPerson.Models.Contracts.Auth
{
	/// <summary>
	/// Описание настроек для токена аутентификации.
	/// </summary>
	public class JwtOptions
	{
		/// <summary>
		/// Название секции в appsettings.json
		/// </summary>
		public const string AppSettingsSection = "JwtOptions";
		/// <summary>
		/// Издатель токена.
		/// </ <summary>
		/// издатель токена
		public string Issuer { get; set; }

		/// </summary>
		/// Потребитель токена.
		/// </summary>
		public string Audience { get; set; }

		/// <summary>
		/// Ключ для шифрации.
		/// </summary>
		public string SecretKey { get; set; }

		/// <summary>
		/// Действие токена в часах.
		/// </summary>
		public int ExpireHours { get; set; }
	}
}
