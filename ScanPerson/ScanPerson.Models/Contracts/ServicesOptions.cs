/// <summary>
/// Настройки используемых сервисов.
/// </summary>
public class ServicesOptions
{
	/// <summary>
	/// Название секции в appsettings.json
	/// </summary>
	public const string AppSettingsSection = "ServicesOptions";

	/// <summary>
	/// Список отключенных сервисов.
	/// </summary>
	public string[] UnUsingServices { get; set; }
}