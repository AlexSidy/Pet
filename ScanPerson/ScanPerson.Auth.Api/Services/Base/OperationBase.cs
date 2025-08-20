using ScanPerson.Models.Responses;

namespace ScanPerson.Auth.Api.Services.Base
{
	public abstract class OperationBase
	{
		private const string ErrorDefault = "An error occurred while performing the operation.";

		protected static ScanPersonResponse GetSuccess()
		{
			return new ScanPersonResponse();
		}

		protected static ScanPersonResponse GetSuccess<TResult>(TResult result) where TResult : class
		{
			return new ScanPersonResultResponse<TResult>(result);
		}

		protected static ScanPersonResponse GetFail(string error = ErrorDefault)
		{
			return new ScanPersonResponse(error);
		}

		protected static ScanPersonResponse GetFail(IEnumerable<string> errors)
		{
			return new ScanPersonResponse(errors);
		}
	}
}
