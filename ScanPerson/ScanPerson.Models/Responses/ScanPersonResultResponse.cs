namespace ScanPerson.Models.Responses
{
	/// <summary>
	/// Response from operation with result object.
	/// </summary>
	public class ScanPersonResultResponse<TResult>: ScanPersonResponse
		where TResult : class
	{
		public ScanPersonResultResponse(TResult result) : base()
		{
			Result = result;
		}

		/// <summary>
		/// Result of success operation.
		/// </summary>
		public TResult Result { get; set; }
	}
}
