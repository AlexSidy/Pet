using ScanPerson.Models.Items;
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

		protected static ScanPersonResponseBase GetAggregatedResult<TResult>(TResult[] results)
			where TResult : ScanPersonResponseBase
		{
			var errors = results
				.Where(x => !x.IsSuccess)
				.Select(x => x.Error)
				.Where(e => !string.IsNullOrEmpty(e));

			if (results.Any(x => x.IsSuccess))
			{
				return GetSuccess(
					results
						.Where(x => x.IsSuccess)
						.OfType<ScanPersonResultResponse<PersonInfoItem>>()
						.Where(x => x != null)
						.Select(x => x!.Result)
						.ToHashSet()
						.ToArray(),
					errors);
			}

			return GetFail(errors);
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
			if (errors.Any())
			{
				return new ScanPersonResponseBase(errors);
			}

			return GetFail();
		}
	}
}
