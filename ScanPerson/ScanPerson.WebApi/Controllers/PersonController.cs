using Microsoft.AspNetCore.Mvc;
using ScanPerson.BusinessLogic.Services;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Items;

namespace ScanPerson.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public PersonController(ILogger<TestController> logger)
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
    }
}
