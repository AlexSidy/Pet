using System.Collections.Generic;
using System.Linq;

namespace ScanPerson.Models.Responses
{
	/// <summary>
	/// Response from operation.
	/// </summary>
	public class ScanPersonResponseBase
	{
		/// <summary>
		/// Constructor for success response.
		/// </summary>
		public ScanPersonResponseBase()
		{
			IsSuccess = true;
			Errors = Enumerable.Empty<string>();
		}

		/// <summary>
		/// Constructor for unsuccess response with errors.
		/// </summary>
		public ScanPersonResponseBase(IEnumerable<string> errors)
		{
			IsSuccess = false;
			Errors = errors;
		}

		/// <summary>
		/// Constructor for unsuccess response with error.
		/// </summary>
		public ScanPersonResponseBase(string error)
		{
			IsSuccess = false;
			Errors = new[] { error };
		}

		/// <summary>
		/// Success marker.
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// Aggregate errors
		/// </summary>
		public string Error
		{
			get
			{
				return Errors == null ? null : string.Join(", ", Errors);
			}
		}

		/// <summary>
		/// List of errors.
		/// </summary>
		private IEnumerable<string> Errors { get; set; }
	}
}
