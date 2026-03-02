using AhorraYa.Application.Dtos.City;
using AhorraYa.Entities;
using AutoMapper;

namespace AhorraYa.WebApi.Mapping
{
    public class CityMappingProfile : Profile
    {
        public CityMappingProfile()
        {
                CreateMap<City, CityResponseDto>()
                .ForMember(dest => dest.ProvinceName,
                opt => opt.MapFrom(src => src.Province!.ProvinceName))
                .ForMember(dest => dest.CountryId,
                opt => opt.MapFrom(src => src.Province!.CountryId))
                .ForMember(dest => dest.CountryName,
                opt => opt.MapFrom(src => src.Province!.Country!.CountryName)).ReverseMap();
                CreateMap<CityRequestDto, City>();
        }
    }
}
