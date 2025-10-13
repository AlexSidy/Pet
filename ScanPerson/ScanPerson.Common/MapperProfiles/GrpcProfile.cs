using AutoMapper;

using ScanPerson.Common.GrpcServices;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.Common.MapperProfiles
{
	/// <summary>
	/// Mapping profile for preparing data of response.
	/// </summary>
	public class GrpcProfile : Profile
	{
		public GrpcProfile()
		{
			CreateMap<GrpcPersonInfoRequest, PersonInfoRequest>();
			CreateMap<ScanPersonResponseBase, GrpcScanPersonResponse>()
				.ForMember(dest => dest.Error, opt => opt.MapFrom(src => src.Error ?? string.Empty))
				.ForMember(dest => dest.Result, opt => opt.Ignore());
			CreateMap<ScanPersonResultResponse<PersonInfoItem>, GrpcScanPersonResponse>()
				.ForMember(dest => dest.Error, opt => opt.MapFrom(src => src.Error ?? string.Empty));
			CreateMap<PersonInfoItem, GrpcPersonInfoItem>()
				.ForMember(dest => dest.Mail, opt => opt.MapFrom(src => src.Mail ?? string.Empty));
			CreateMap<LocationItem, GrpcLocationItem>()
				.ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.CountryName ?? string.Empty))
				.ForMember(dest => dest.CurrentRegion, opt => opt.MapFrom(src => src.CurrentRegion ?? string.Empty))
				.ForMember(dest => dest.RegistrationOkrug, opt => opt.MapFrom(src => src.RegistrationOkrug ?? string.Empty))
				.ForMember(dest => dest.RegistrationCapital, opt => opt.MapFrom(src => src.RegistrationCapital ?? string.Empty))
				.ForMember(dest => dest.OperatorCity, opt => opt.MapFrom(src => src.OperatorCity ?? string.Empty))
				.ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.OperatorName ?? string.Empty));

			CreateMap<GrpcScanPersonResponse, ScanPersonResponseBase>()
				.ForMember(dest => dest.Error, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Error) ? null : src.Error));
			CreateMap<GrpcScanPersonResponse, ScanPersonResultResponse<PersonInfoItem>>()
				.ForMember(dest => dest.Error, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Error) ? null : src.Error));
			CreateMap<GrpcPersonInfoItem, PersonInfoItem>()
				.ForMember(dest => dest.Mail, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Mail) ? null : src.Mail));
			CreateMap<GrpcLocationItem, LocationItem>()
				.ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.CountryName) ? null : src.CountryName))
				.ForMember(dest => dest.CurrentRegion, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.CurrentRegion) ? null : src.CurrentRegion))
				.ForMember(dest => dest.RegistrationOkrug, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.RegistrationOkrug) ? null : src.RegistrationOkrug))
				.ForMember(dest => dest.RegistrationCapital, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.RegistrationCapital) ? null : src.RegistrationCapital))
				.ForMember(dest => dest.OperatorCity, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.OperatorCity) ? null : src.OperatorCity))
				.ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.OperatorName) ? null : src.OperatorName));
		}
	}
}
