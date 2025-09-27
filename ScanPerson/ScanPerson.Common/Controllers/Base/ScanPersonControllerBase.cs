using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ScanPerson.Models.Responses;

namespace ScanPerson.Common.Controllers
{
	/// <summary>
	/// Base controller.
	/// </summary>
	[Controller]
	public class ScanPersonControllerBase : ControllerBase
	{
		protected static IResult GetResult<TResult>(TResult response, IResult? succces = null, IResult? fail = null)
			where TResult : ScanPersonResponseBase?
		{
			return response!.IsSuccess
				? succces ?? Results.Ok(response)
				: fail ?? Results.BadRequest(response.Error);
		}
	}
}
