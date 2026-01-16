using AhorraYa.Application.Dtos.Product;
using AhorraYa.Entities;
using AutoMapper;

namespace AhorraYa.WebApi.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName, 
                opt => opt.MapFrom(src => src.Category!.CategoryName))
                .ForMember(dest => dest.BarCode,
                opt => opt.MapFrom(src => src.BarCode))
                .ForMember(dest => dest.BrandName, 
                opt => opt.MapFrom(src => src.Brand!.BrandName));
            CreateMap<ProductRequestDto, Product>()
                                .ForMember(dest => dest.BarCode,
                opt => opt.MapFrom(src => src.BarCode))
                .ForMember(dest => dest.CategoryId,
                opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.BrandId,
                opt => opt.MapFrom(src => src.BrandId));
        }
    }
}
