using ScanPerson.Models.Responses;

namespace ScanPerson.Auth.Api.Services.Base
{
	public abstract class OperationBase
	{
		private const string ErrorDefault = "An error occurred while performing the operation.";

		protected ScanPersonResponse GetSuccess()
		{
			return new ScanPersonResponse();
		}

		protected ScanPersonResponse GetSuccess<TResult>(TResult result) where TResult : class
		{
			return new ScanPersonResultResponse<TResult>(result);
		}

		protected ScanPersonResponse GetFail(string error = ErrorDefault)
		{
			return new ScanPersonResponse(error);
		}

		protected ScanPersonResponse GetFail(IEnumerable<string> errors)
		{
			return new ScanPersonResponse(errors);
		}
	}
}
