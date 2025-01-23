using Microsoft.AspNetCore.Mvc;
using ScanPerson.Models.Responses;

namespace ScanPerson.Auth.Api.Controllers
{
	[Controller]
	public class AuthControllerBase : ControllerBase
	{
		protected IResult GetResult(ScanPersonResponse response, IResult? succces = null, IResult? fail = null)
		{
			return response.IsSuccess
				? succces == null ? Results.Ok(response) : succces
				: fail == null ? Results.BadRequest(response.Error): fail;
		}
	}
}
