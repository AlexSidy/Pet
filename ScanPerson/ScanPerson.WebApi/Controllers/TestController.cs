using Microsoft.AspNetCore.Mvc;

namespace ScanPerson.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = nameof(GetItems))]
        public IEnumerable<TestItem> GetItems()
        {
            return Enumerable.Range(1, 5).Select(index => new TestItem
            {
                Id = index + 10 + 15,
                Name = "NameTest1111" + index
            })
            .ToArray();
        }
    }
}
