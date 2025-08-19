using System.Reflection;

using Microsoft.Extensions.Logging;

using ScanPerson.DAL2.Contexts;
using ScanPerson.DAL2.Initializers.Interfaces;

using ScanPerson.Common.Resources;
using ScanPerson.Models.Entities;

namespace ScanPerson.DAL2.Initializers
{
	internal class PersonInitializer(
		ILogger<PersonInitializer> logger,
		ScanPersonDbContext context) : IInitializer
	{
		public void Seed()
		{
			try
			{
				logger.LogInformation(string.Format(Messages.StartedMethod, MethodBase.GetCurrentMethod()));
				if (context.Persons?.Any() != true)
				{
					var persons = new[]
					{
					new PersonEntity { Name = "John Doe", Mail = "JohnDoe@mail.ru" },
					new PersonEntity { Name = "Jane Smith", Mail = "JaneSmith@mail.ru" }
				};

					context!.Persons!.AddRange(persons);
					context.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, Messages.InitDataError);
				// Add transaction rollback #35
			}

		}
	}
}
