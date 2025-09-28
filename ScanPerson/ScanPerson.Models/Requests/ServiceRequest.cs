using System;

namespace ScanPerson.Models.Requests
{
	/// <summary>
	/// Input data for retrieving information about a person with a specific service.
	/// </summary>
	public class ServiceRequest
	{
		public ServiceRequest(PersonInfoRequest request, Type serviceTytpe)
		{
			Request = request;
			ServiceTytpe = serviceTytpe;
		}

		/// <summary>
		/// Person info request.
		/// </summary>
		public PersonInfoRequest Request { get; set; }

		/// <summary>
		/// The type of service.
		/// </summary>
		public Type ServiceTytpe { get; set; }
	}
}