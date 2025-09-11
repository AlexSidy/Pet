using System.Collections.Generic;

namespace ScanPerson.Models.Responses
{
	/// <summary>
	/// Response from operation with result object.
	/// </summary>
	public class ScanPersonResultResponse<TResult> : ScanPersonResponseBase
		where TResult : class
	{
		/// <summary>
		/// Parametreless constructor.
		/// </summary>
		public ScanPersonResultResponse() : base()
		{
		}

		public ScanPersonResultResponse(string error) : base(error)
		{
		}

		public ScanPersonResultResponse(TResult result) : base()
		{
			Result = result;
		}

		/// <summary>
		/// Constructor for unsuccess response with errors.
		/// </summary>
		public ScanPersonResultResponse(IEnumerable<string> errors) : base(errors)
		{
		}

		/// <summary>
		/// Result of success operation.
		/// </summary>
		public TResult Result { get; set; }
	}
}
