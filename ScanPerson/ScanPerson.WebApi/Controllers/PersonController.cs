using Microsoft.AspNetCore.Mvc;
using ScanPerson.BusinessLogic.Services;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Items;

namespace ScanPerson.WebApi.Controllers
{
	[ApiController]
	[Route(Program.WebApi + "/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> _logger;

        public PersonController(ILogger<PersonController> logger)
        {
            _logger = logger;
		}

		[HttpGet(nameof(GetPersons))]
        public IEnumerable<PersonItem>? GetPersons(
            [FromQuery] PersonRequest request,
            [FromServices] IPersonService service)
		{
            return service.Query(request);
        }

		[HttpPost(nameof(CreatePerson))]
		public IEnumerable<PersonItem>? CreatePerson(
			[FromBody] PersonRequest request,
			[FromServices] IPersonService service)
		{
			return service.Query(request);
		}
	}
}
