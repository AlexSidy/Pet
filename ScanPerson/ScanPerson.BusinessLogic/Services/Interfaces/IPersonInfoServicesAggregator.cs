using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services.Interfaces
{
	/// <summary>
	/// The service responsible for processing requests from all connected services.
	/// </summary>
	public interface IPersonInfoServicesAggregator
	{
		/// <summary>
		/// Method for scanning information about a person from all services.
		/// </summary>
		/// <param name="request">Request with input data.</param>
		/// <returns>List of information suitable for the input data.</returns>
		Task<ScanPersonResponseBase[]> GetScanPersonInfoAsync(PersonInfoRequest request);
	}
}
