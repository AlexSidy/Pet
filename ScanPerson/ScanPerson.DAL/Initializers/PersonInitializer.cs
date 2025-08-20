using System.Reflection;

using Microsoft.Extensions.Logging;

using ScanPerson.Common.Resources;
using ScanPerson.DAL.Contexts;
using ScanPerson.DAL.Initializers.Interfaces;
using ScanPerson.Models.Entities;

namespace ScanPerson.DAL.Initializers
{
	internal class PersonInitializer(
		ILogger<PersonInitializer> logger,
		ScanPersonDbContext context) : IInitializer
	{
		public void Seed()
		{
			try
			{
				logger.LogInformation(Messages.StartedMethod, MethodBase.GetCurrentMethod());
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
