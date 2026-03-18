using AhorraYa.Application.Dtos.PriceOfShop;
using AhorraYa.Entities;
using AutoMapper;

namespace AhorraYa.WebApi.Mapping
{
    public class PriceOfShopMappingProfile : Profile
    {
        public PriceOfShopMappingProfile()
        {
            CreateMap<PriceOfShop, PriceOfShopResponseDto>()
                .ForMember(dest => dest.Product,
                opt => opt.MapFrom(src => src.Product!.Name))
                .ForMember(dest => dest.ProductImage,
                opt => opt.MapFrom(src => src.Product.Image))
                .ForMember(dest => dest.Brand,
                opt => opt.MapFrom(src => src.Product!.Brand!.BrandName))
                .ForMember(dest => dest.Shop, 
                opt => opt.MapFrom(src => $"{src.Shop.ShopName}: {src.Shop.Location.GetFullAddress()}"))
                .ForMember(dest => dest.ShopName, 
                opt => opt.MapFrom(dest => dest.Shop.ShopName))
                .ForMember(dest => dest.LastModification,
                opt => opt.MapFrom(src => src.RegistrationDate.ToShortDateString()));

            CreateMap<PriceOfShopRequestDto, PriceOfShop>();
            CreateMap<UpdatePriceOfShopRequestDto, PriceOfShop>();

        }
    }
}
