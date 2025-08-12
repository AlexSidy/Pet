using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services
{
	public interface IPersonService
	{
		ScanPersonResultResponse<PersonItem[]?> Query(PersonRequest request);

		ScanPersonResultResponse<PersonItem?>? Find(PersonRequest request);
	}
}
