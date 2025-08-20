using System.Reflection;

using Microsoft.Extensions.Logging;

using ScanPerson.Common.Resources;
using ScanPerson.DAL.Contexts;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services
{
	public class PersonService(
		ILogger<PersonService> logger,
		ScanPersonDbContext context) : IPersonService
	{
		public Task<ScanPersonResultResponse<PersonItem[]?>> QueryAsync(PersonRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, MethodBase.GetCurrentMethod());
			var result = new ScanPersonResultResponse<PersonItem[]?>(context.Persons?.Select(x => new PersonItem(x.Id, x.Name, x.Mail))?.ToArray());

			return Task.FromResult(result);
		}

		public Task<ScanPersonResultResponse<PersonItem?>?> FindAsync(PersonRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, MethodBase.GetCurrentMethod());
			var result = context.Persons?
				.Select(x => new ScanPersonResultResponse<PersonItem?>(new PersonItem(x.Id, x.Name, x.Mail)))
				.FirstOrDefault();

			return Task.FromResult(result);
		}
	}
}
