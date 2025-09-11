using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services.Interfaces
{
	/// <summary>
	/// Service for getting information about a person.
	/// </summary>
	public interface IPersonInfoService
	{
		/// <summary>
		/// Getting information about a person.
		/// </summary>
		/// <param name="request">Request with input data.</param>
		/// <returns>Result with output data.</returns>
		Task<ScanPersonResponseBase> GetInfoAsync(PersonInfoRequest request);

		/// <summary>
		/// Checking whether the service logic needs to be run.
		/// </summary>
		/// <returns>True if the service logic needs to be run.</returns>
		bool CanAccept();
	}
}
