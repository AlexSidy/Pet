using Scanperson.DAL.Contexts;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;

namespace ScanPerson.BusinessLogic.Services
{
	public class PersonService: IPersonService
	{
		private readonly ScanPersonDbContext _context;

		public PersonService(ScanPersonDbContext context)
		{
			_context = context;
		}

		public PersonItem[]? Query(PersonRequest request)
		{
			var result = _context.Persons?.Select(x => new PersonItem(x.Id, x.Name, x.Mail))?.ToArray();

			return result;
		}

		public PersonItem? Find(PersonRequest request)
		{
			var result = _context.Persons?.Select(x => new PersonItem(x.Id, x.Name, x.Mail)).FirstOrDefault();

			return result;
		}
	}
}
