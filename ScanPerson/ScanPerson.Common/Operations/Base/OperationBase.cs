using ScanPerson.Models.Responses;

namespace ScanPerson.Common.Operations.Base
{
	/// <summary>
	/// Base class for all operations ScanPerson services.
	/// </summary>
	public abstract class OperationBase
	{
		private const string ErrorDefault = "An error occurred while performing the operation.";

		protected static ScanPersonResponseBase GetSuccess()
		{
			return new ScanPersonResponseBase();
		}

		protected static ScanPersonResponseBase GetSuccess<TResult>(TResult result, IEnumerable<string>? warnings = null) where TResult : class
		{
			return new ScanPersonResultResponse<TResult>(result, warnings);
		}

		protected static TResult GetSuccess<TItem, TResult>(TItem item, IEnumerable<string>? warnings = null)
			where TItem : class
			where TResult : ScanPersonResultResponse<TItem>
		{
			return (TResult)new ScanPersonResultResponse<TItem>(item, warnings);
		}

		protected static ScanPersonResponseBase GetFail(string error = ErrorDefault)
		{
			return new ScanPersonResponseBase(error);
		}

		protected static ScanPersonResponseBase GetFail<TResult>(string error = ErrorDefault) where TResult : class
		{
			return new ScanPersonResultResponse<TResult>(error);
		}

		protected static TResult GetFail<TItem, TResult>(string error = ErrorDefault)
			where TItem : class
			where TResult : ScanPersonResultResponse<TItem>
		{
			return (TResult)new ScanPersonResultResponse<TItem>(error);
		}

		protected static ScanPersonResponseBase GetFail(IEnumerable<string> errors)
		{
			if (errors.Any())
			{
				return new ScanPersonResponseBase(errors);
			}

			return GetFail();
		}
	}
}
