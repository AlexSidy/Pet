using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;

namespace ScanPerson.BusinessLogic.Services
{
	public interface IPersonService
	{
		PersonItem[]? Query(PersonRequest request);
	}
}
