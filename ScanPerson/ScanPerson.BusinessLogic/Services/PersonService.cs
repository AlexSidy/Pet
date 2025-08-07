using System.Reflection;

using Microsoft.Extensions.Logging;

using Scanperson.DAL.Contexts;

using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;

namespace ScanPerson.BusinessLogic.Services
{
	public class PersonService(
		ILogger<PersonService> logger,
		ScanPersonDbContext context) : IPersonService
	{
		public PersonItem[]? Query(PersonRequest request)
		{
			logger.LogInformation(string.Format(Messages.StartedMethod, MethodBase.GetCurrentMethod()));
			var result = context.Persons?.Select(x => new PersonItem(x.Id, x.Name, x.Mail))?.ToArray();

			return result;
		}

		public PersonItem? Find(PersonRequest request)
		{
			logger.LogInformation(string.Format(Messages.StartedMethod, MethodBase.GetCurrentMethod()));
			var result = context.Persons?.Select(x => new PersonItem(x.Id, x.Name, x.Mail)).FirstOrDefault();

			return result;
		}
	}
}
