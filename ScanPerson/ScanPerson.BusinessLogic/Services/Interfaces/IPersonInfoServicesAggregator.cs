using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services.Interfaces
{
	/// <summary>
	/// Сервис отвечающий за обработку запросов со всех подключенных сервисов
	/// </summary>
	public interface IPersonInfoServicesAggregator
	{
		/// <summary>
		/// Метод для сканирования информации о человеке со всех сервисов.
		/// </summary>
		/// <param name="request">Запрос с входящими данными.</param>
		/// <returns>Список информации подходящих по начальным данным.</returns>
		Task<ScanPersonResponseBase[]> GetScanPersonInfoAsync(PersonInfoRequest request);
	}
}
