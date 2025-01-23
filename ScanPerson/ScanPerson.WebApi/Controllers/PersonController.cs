using Microsoft.AspNetCore.Mvc;
using ScanPerson.BusinessLogic.Services;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Items;

namespace ScanPerson.WebApi.Controllers
{
    [ApiController]
    [Route("webApi/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> _logger;

        public PersonController(ILogger<PersonController> logger)
        {
            _logger = logger;
        }

		[HttpGet(Name = nameof(Query))]
        public IEnumerable<PersonItem>? Query(
            [FromQuery] PersonRequest request,
            [FromServices] IPersonService service)
		{
            return service.Query(request);
        }

		[HttpPost(Name = nameof(Post))]
		public IEnumerable<PersonItem>? Post(
			[FromBody] PersonRequest request,
			[FromServices] IPersonService service)
		{
			return service.Query(request);
		}
	}
}
