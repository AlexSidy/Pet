namespace ScanPerson.Auth.Api.Initializers.Interfaces;

// Интерфейс для первичной инициализации данных в сервисе.
internal interface IInitializer
{
	/// <summary>
	/// Метод для запроса данных, их проверки и дальнейшей инициализации в случае необходимости.
	/// </summary>
	/// <returns></returns>
	Task SeedAsync();
}
