using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ScanPerson.Models.Responses;

namespace ScanPerson.Common.Controllers
{
	[Controller]
	public class ScanPersonControllerBase : ControllerBase
	{
		protected IResult GetResult(ScanPersonResponse response, IResult? succces = null, IResult? fail = null)
		{
			return response.IsSuccess
				? succces ?? Results.Ok(response)
				: fail ?? Results.BadRequest(response.Error);
		}
	}
}
