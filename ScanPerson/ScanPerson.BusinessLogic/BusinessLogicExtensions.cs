using Microsoft.Extensions.DependencyInjection;
using ScanPerson.BusinessLogic.Services;

namespace ScanPerson.BusinessLogic
{
	public static class BusinessLogicExtensions
	{
		public static void AddBusinessLogicServices(this IServiceCollection services)
		{
			services.AddTransient<IPersonService, PersonService>();
		}
	}
}
