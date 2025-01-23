using Scanperson.DAL.Contexts;
using Scanperson.DAL.Initializers.Interfaces;
using ScanPerson.Models.Entities;

namespace Scanperson.DAL.Initializers
{
	internal class PersonInitializer(ScanPersonDbContext context) : IInitializer
	{
		private readonly ScanPersonDbContext _context = context;

		public void Seed()
		{
			if (_context.Persons?.Any() != true)
			{
				var persons = new[]
				{
					new PersonEntity { Name = "John Doe", Mail = "JohnDoe@mail.ru" },
					new PersonEntity { Name = "Jane Smith", Mail = "JaneSmith@mail.ru" }
				};

				_context!.Persons!.AddRange(persons);
				_context.SaveChanges();
			}
		}
	}
}
