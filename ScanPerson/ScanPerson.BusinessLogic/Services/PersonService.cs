using System.Reflection;

using Microsoft.Extensions.Logging;


using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services
{
	public class PersonService(
		ILogger<PersonService> logger
		) : IPersonService
	{
		public ScanPersonResultResponse<PersonItem[]?> Query(PersonRequest request)
		{
			logger.LogInformation(string.Format(Messages.StartedMethod, MethodBase.GetCurrentMethod()));
			//var result = new ScanPersonResultResponse<PersonItem[]?>(context.Persons?.Select(x => new PersonItem(x.Id, x.Name, x.Mail))?.ToArray());
			var result = new ScanPersonResultResponse<PersonItem[]?>([new PersonItem(1, "x.Name", "x.Mail")]);

			return result;
		}

		public ScanPersonResultResponse<PersonItem?>? Find(PersonRequest request)
		{
			logger.LogInformation(string.Format(Messages.StartedMethod, MethodBase.GetCurrentMethod()));
			//var result = context.Persons?
			//	.Select(x => new ScanPersonResultResponse<PersonItem?>(new PersonItem(x.Id, x.Name, x.Mail)))
			//	.FirstOrDefault();
			var result = new ScanPersonResultResponse<PersonItem?>(new PersonItem(1, "x.Name", "x.Mail"));

			return result;
		}
	}
}
