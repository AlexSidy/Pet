using AutoMapper;

using ScanPerson.Models.Items;

namespace ScanPerson.BusinessLogic.MapperProfiles
{
	/// <summary>
	/// Профиль маппинга контрактов, предназначенных для отображения на стороне клиента.
	/// </summary>
	public class DisplayProfile : Profile
	{
		public DisplayProfile()
		{
			CreateMap<LocationDeserialized, LocationItem>()
				.ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
				.ForMember(dest => dest.CurrentRegion, opt => opt.MapFrom(src => src.Region.Name))
				.ForMember(dest => dest.RegistrationOkrug, opt => opt.MapFrom(src => src.Okrug))
				.ForMember(dest => dest.RegistrationCapital, opt => opt.MapFrom(src => src.Capital.Name))
				.ForMember(dest => dest.OperatorCity, opt => opt.MapFrom(src => src.Operator.Name))
				.ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.Operator.OperBrand));
		}
	}
}
