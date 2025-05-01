using Microsoft.AspNetCore.Mvc;
using ScanPerson.BusinessLogic.Services;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Items;
using Microsoft.AspNetCore.Authorization;

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
		[Authorize]
        public IEnumerable<PersonItem>? GetPersons(
            [FromQuery] PersonRequest request,
            [FromServices] IPersonService service)
		{
            return service.Query(request);
        }

		[HttpPost(nameof(GetPerson))]
		public PersonItem? GetPerson(
			[FromBody] PersonRequest request,
			[FromServices] IPersonService service)
		{
			return service.Find(request);
		}
	}
}
