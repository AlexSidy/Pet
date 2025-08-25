using ScanPerson.Models.Responses;

namespace ScanPerson.Common.Operations.Base
{
	public abstract class OperationBase
	{
		private const string ErrorDefault = "An error occurred while performing the operation.";

		protected static ScanPersonResponseBase GetSuccess()
		{
			return new ScanPersonResponseBase();
		}

		protected static ScanPersonResponseBase GetSuccess<TResult>(TResult result) where TResult : class
		{
			return new ScanPersonResultResponse<TResult>(result);
		}

		protected static ScanPersonResponseBase GetFail(string error = ErrorDefault)
		{
			return new ScanPersonResponseBase(error);
		}

		protected static ScanPersonResponseBase GetFail<TResult>(string error = ErrorDefault) where TResult : class
		{
			return new ScanPersonResultResponse<TResult>(error);
		}

		protected static ScanPersonResponseBase GetFail(IEnumerable<string> errors)
		{
			return new ScanPersonResponseBase(errors);
		}
	}
}
