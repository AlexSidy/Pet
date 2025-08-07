using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.Common.Controllers;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.WebApi.Controllers
{
	[ApiController]
	[Route(Program.WebApi + "/[controller]")]
	public class PersonController(ILogger<PersonController> logger) : ScanPersonControllerBase
	{
		[HttpGet(nameof(GetPersonsAsync))]
		[Authorize]
		public async Task<IResult> GetPersonsAsync(
			[FromQuery] PersonRequest request,
			[FromServices] IPersonService service)
		{
			logger.LogInformation(string.Format(Messages.StartedMethod, MethodBase.GetCurrentMethod()));
			var result = new ScanPersonResultResponse<IEnumerable<PersonItem>?>(service.Query(request));

			return GetResult(result);
		}

		[HttpPost(nameof(GetPersonAsync))]
		public async Task<IResult> GetPersonAsync(
			[FromBody] PersonRequest request,
			[FromServices] IPersonService service)
		{
			logger.LogInformation(string.Format(Messages.StartedMethod, MethodBase.GetCurrentMethod()));
			var result = new ScanPersonResultResponse<PersonItem?>(service.Find(request));

			return GetResult(result);
		}
	}
}
