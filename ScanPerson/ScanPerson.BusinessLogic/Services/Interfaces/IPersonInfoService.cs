using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services.Interfaces
{
	/// <summary>
	/// Сервис для получения информации о человеке.
	/// </summary>
	public interface IPersonInfoService
	{
		/// <summary>
		/// Получение информации о человеке.
		/// </summary>
		/// <param name="request">Запрос с входящими данными.</param>
		/// <returns>Результат с полученной информацией.</returns>
		Task<ScanPersonResponseBase> GetInfoAsync(PersonInfoRequest request);

		/// <summary>
		/// Проверка нужно ли запускать логику сервиса.
		/// </summary>
		/// <param name="serviceOptions">Настройки для сервисов.</param>
		/// <returns></returns>
		bool CanAccept();
	}
}
