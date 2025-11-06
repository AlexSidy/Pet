using ScanPerson.Models.Items;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services.Interfaces
{

	/// <summary>
	/// Service to work with AI chat.
	/// </summary>
	//TODO: Будет вынесен в отдельный docker сервис в #97
	public interface IChatAIService
	{
		/// <summary>
		/// Get identification from AI.
		/// </summary>
		/// <param name="names"></param>
		/// <returns></returns>
		Task<ScanPersonResultResponse<PersonIdentificationItem>> GetExactIdentificationAsync(string[] names);
	}
}
