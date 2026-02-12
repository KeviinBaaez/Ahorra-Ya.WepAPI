using AhorraYa.Application.Dtos.Country;
using AhorraYa.Entities;
using AutoMapper;

namespace AhorraYa.WebApi.Mapping
{
    public class CountryMappingProfile : Profile
    {
        public CountryMappingProfile()
        {
            CreateMap<Country, CountryResponseDto>();
            CreateMap<CountryRequestDto, Country>();
        }
    }
}
