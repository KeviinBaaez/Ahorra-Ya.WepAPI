using AhorraYa.Application.Dtos.Shop;
using AhorraYa.Entities;
using AutoMapper;

namespace AhorraYa.WebApi.Mapping
{
    public class ShopMappingProfile : Profile
    {
        public ShopMappingProfile()
        {
            CreateMap<Shop, ShopResponseDto>()
                .ForMember(dest => dest.Address, 
                opt => opt.MapFrom(src => src.Location!.GetFullAddress()))
                .ForMember(dest => dest.City,
                opt => opt.MapFrom(src => src.Location!.City!.CityName));
            CreateMap<ShopRequestDto, Shop>();
        }
    }
}
