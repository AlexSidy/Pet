using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Controllers;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Requests;

namespace ScanPerson.WebApi.Controllers
{
	[ApiController]
	[Route(Program.WebApi + "/[controller]")]
	public class PersonInfoController(ILogger<PersonInfoController> logger, IPersonInfoServicesAggregator service) : ScanPersonControllerBase
	{
		[Authorize]
		[HttpPost(nameof(GetScanPersonInfoAsync))]
		public async Task<IResult> GetScanPersonInfoAsync([FromBody] PersonInfoRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, ControllerContext?.RouteData?.Values["action"]);
			var result = await service.GetScanPersonInfoAsync(request);

			return GetResult(result.ToHashSet().FirstOrDefault());
		}

		[HttpPost(nameof(GetSimpleAsync))]
		public async Task<IResult> GetSimpleAsync([FromBody] PersonInfoRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, ControllerContext?.RouteData?.Values["action"]);
			var result = await Task.FromResult(request);

			return Results.Ok(result);
		}
	}
}
