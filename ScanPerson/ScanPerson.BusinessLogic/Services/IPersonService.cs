using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services
{
	public interface IPersonService
	{
		Task<ScanPersonResultResponse<PersonItem[]?>> QueryAsync(PersonRequest request);

		Task<ScanPersonResultResponse<PersonItem?>?> FindAsync(PersonRequest request);
	}
}
