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
			SetupGprcToProjectModelsMapping();
			SetupProjectModelsToGprcMapping();
		}

		/// <summary>
		/// Check and get null if empty string.
		/// </summary>
		/// <param name="value">Source field value.</param>
		/// <returns>Value or null if source is null or empty.</returns>
		private static string? GetNullIfNullOrEmpty(string? value)
		{
			return string.IsNullOrEmpty(value) ? null : value;
		}

		/// <summary>
		/// Check and get empty if value is null.
		/// </summary>
		/// <param name="value">Source field value.</param>
		/// <returns>Value or empty if source is null.</returns>
		private static string? GetEmptyIfNull(string? value)
		{
			return value ?? string.Empty;
		}

		/// <summary>
		/// Configurate mapping project entities to gprc enttities.
		/// </summary>
		private void SetupProjectModelsToGprcMapping()
		{
			CreateMap<GrpcPersonInfoRequest, PersonInfoRequest>();
			CreateMap<GrpcScanPersonResponse, ScanPersonResponseBase>()
				.ForMember(dest => dest.Error, opt => opt.MapFrom(src => GetNullIfNullOrEmpty(src.Error)));
			CreateMap<GrpcScanPersonResponse, ScanPersonResultResponse<PersonInfoItem>>()
				.ForMember(dest => dest.Error, opt => opt.MapFrom(src => GetNullIfNullOrEmpty(src.Error)));
			CreateMap<GrpcPersonInfoItem, PersonInfoItem>()
				.ForMember(dest => dest.Mail, opt => opt.MapFrom(src => GetNullIfNullOrEmpty(src.Mail)));
			CreateMap<GrpcLocationItem, LocationItem>()
				.ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => GetNullIfNullOrEmpty(src.CountryName)))
				.ForMember(dest => dest.CurrentRegion, opt => opt.MapFrom(src => GetNullIfNullOrEmpty(src.CurrentRegion)))
				.ForMember(dest => dest.RegistrationOkrug, opt => opt.MapFrom(src => GetNullIfNullOrEmpty(src.RegistrationOkrug)))
				.ForMember(dest => dest.RegistrationCapital, opt => opt.MapFrom(src => GetNullIfNullOrEmpty(src.RegistrationCapital)))
				.ForMember(dest => dest.OperatorCity, opt => opt.MapFrom(src => GetNullIfNullOrEmpty(src.OperatorCity)))
				.ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => GetNullIfNullOrEmpty(src.OperatorName)));
		}

		/// <summary>
		/// Configurate mapping gprc enttities to project entities.
		/// </summary>
		private void SetupGprcToProjectModelsMapping()
		{
			CreateMap<ScanPersonResponseBase, GrpcScanPersonResponse>()
				.ForMember(dest => dest.Error, opt => opt.MapFrom(src => GetEmptyIfNull(src.Error)))
				.ForMember(dest => dest.Result, opt => opt.Ignore());
			CreateMap<ScanPersonResultResponse<PersonInfoItem>, GrpcScanPersonResponse>()
				.ForMember(dest => dest.Error, opt => opt.MapFrom(src => GetEmptyIfNull(src.Error)));
			CreateMap<PersonInfoItem, GrpcPersonInfoItem>()
				.ForMember(dest => dest.Mail, opt => opt.MapFrom(src => GetEmptyIfNull(src.Mail)));
			CreateMap<LocationItem, GrpcLocationItem>()
				.ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => GetEmptyIfNull(src.CountryName)))
				.ForMember(dest => dest.CurrentRegion, opt => opt.MapFrom(src => GetEmptyIfNull(src.CurrentRegion)))
				.ForMember(dest => dest.RegistrationOkrug, opt => opt.MapFrom(src => GetEmptyIfNull(src.RegistrationOkrug)))
				.ForMember(dest => dest.RegistrationCapital, opt => opt.MapFrom(src => GetEmptyIfNull(src.RegistrationCapital)))
				.ForMember(dest => dest.OperatorCity, opt => opt.MapFrom(src => GetEmptyIfNull(src.OperatorCity)))
				.ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => GetEmptyIfNull(src.OperatorName)));
		}
	}
}
