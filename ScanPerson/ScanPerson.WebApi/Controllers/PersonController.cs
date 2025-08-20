using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.Common.Controllers;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Requests;

namespace ScanPerson.WebApi.Controllers
{
	[ApiController]
	[Route(Program.WebApi + "/[controller]")]
	public class PersonController(ILogger<PersonController> logger, IPersonService service) : ScanPersonControllerBase
	{
		[HttpGet(nameof(GetPersonsAsync))]
		[Authorize]
		[Obsolete("Will be removed or changed.")]
		public async Task<IResult> GetPersonsAsync([FromQuery] PersonRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, ControllerContext?.RouteData?.Values["action"]);
			var result = service.Query(request);

			return GetResult(result);
		}

		[HttpPost(nameof(GetPersonAsync))]
		public async Task<IResult> GetPersonAsync([FromBody] PersonRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, ControllerContext?.RouteData?.Values["action"]);
			var result = service.Find(request);

			return GetResult(result);
		}
	}
}
