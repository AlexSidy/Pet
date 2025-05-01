using Scanperson.DAL.Contexts;
using Scanperson.DAL.Initializers.Interfaces;
using ScanPerson.Models.Entities;

namespace Scanperson.DAL.Initializers
{
	internal class PersonInitializer(ScanPersonDbContext context) : IInitializer
	{
		public void Seed()
		{
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
	}
}
