using AhorraYa.Application.Dtos.Location;
using AhorraYa.Entities;
using AutoMapper;

namespace AhorraYa.WebApi.Mapping
{
    public class LocationMappingProfile : Profile
    {
        public LocationMappingProfile()
        {
            CreateMap<Location, LocationResponseDto>().
                ForMember(dest => dest.CityName, 
                opt => opt.MapFrom(src => src.City!.CityName));
            CreateMap<LocationRequestDto, Location>();
        }
    }
}
