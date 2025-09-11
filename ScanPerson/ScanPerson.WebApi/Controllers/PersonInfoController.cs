using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Controllers;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Requests;

namespace ScanPerson.WebApi.Controllers
{
	/// <summary>
	/// API controller for retrieving person information.
	/// </summary>
	[ApiController]
	[Route(Program.WebApi + "/[controller]")]
	public class PersonInfoController(ILogger<PersonInfoController> logger, IPersonInfoServicesAggregator service) : ScanPersonControllerBase
	{
		/// <summary>
		/// Retrieves person information based on a phone number.
		/// </summary>
		/// <param name="request">The request containing the input data.</param>
		/// <returns>An <see cref="IResult"/> containing the retrieved person information.</returns>
		[Authorize]
		[HttpPost(nameof(GetScanPersonInfoAsync))]
		public async Task<IResult> GetScanPersonInfoAsync([FromBody] PersonInfoRequest request)
		{
			logger.LogInformation(Messages.StartedMethod, ControllerContext?.RouteData?.Values["action"]);
			var result = await service.GetScanPersonInfoAsync(request);

			return GetResult(result.ToHashSet().FirstOrDefault());
		}
	}
}