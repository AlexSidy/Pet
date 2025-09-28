using AutoMapper;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.Models.Requests;


namespace ScanPerson.BusinessLogic.MapperProfiles
{
	/// <summary>
	/// Mapping profile for preparing data of request.
	/// </summary>
	public class RequestProfile : Profile
	{
		public RequestProfile()
		{
			CreateMap<ServiceRequest, PersonInfoRequest>()
				.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => GetPreparedValueByTypeService(src)));
		}

		/// <summary>
		/// Returns the phone number based on the service type.
		/// </summary>
		/// <param name="request">Request object.</param>
		/// <returns>Prepared value.</returns>
		private static string GetPreparedValueByTypeService(ServiceRequest request)
		{
			switch (request.ServiceTytpe)
			{
				case var t when t == typeof(GeoService):
					return "7" + request.Request.PhoneNumber;
				default:
					return request.Request.PhoneNumber;
			}
		}
	}
}
