using AhorraYa.Application.Dtos.Province;
using AhorraYa.Entities;
using AutoMapper;

namespace AhorraYa.WebApi.Mapping
{
    public class ProvinceMappingProfile : Profile
    {
        public ProvinceMappingProfile()
        {
            CreateMap<Province, ProvinceResponseDto>()
                .ForMember(dest => dest.CountryName,
                opt => opt.MapFrom(src => src.Country!.CountryName)).ReverseMap();
            CreateMap<ProvinceRequestDto, Province>();
        }
    }
}
