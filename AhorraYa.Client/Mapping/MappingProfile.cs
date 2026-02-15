using AhorraYa.Application.Dtos.Brand;
using AhorraYa.Application.Dtos.Category;
using AhorraYa.Application.Dtos.Location;
using AhorraYa.Application.Dtos.Product;
using AhorraYa.Application.Dtos.Shop;
using AhorraYa.WebClient.ViewModels.Brands;
using AhorraYa.WebClient.ViewModels.Categories;
using AhorraYa.WebClient.ViewModels.Locations;
using AhorraYa.WebClient.ViewModels.Product;
using AhorraYa.WebClient.ViewModels.Shops;
using AutoMapper;

namespace AhorraYa.WebClient.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            LoadCategoryMapping();
            LoadBrandMapping();
            LoadProductMapping();
            LoadLocationMapping();
            LoadShopMapping();
        }

        private void LoadShopMapping()
        {
            CreateMap<ShopRequestDto, ShopEditVm>().ReverseMap();
            CreateMap<ShopResponseDto, ShopListVm>().ReverseMap();
        }

        private void LoadLocationMapping()
        {
            CreateMap<LocationRequestDto, LocationEditVm>().ReverseMap();
            CreateMap<LocationResponseDto, LocationListVm>().ReverseMap();
        }

        private void LoadProductMapping()
        {
            CreateMap<ProductRequestDto, ProductEditVm>().ReverseMap();
            CreateMap<ProductResponseDto, ProductListVm>().ReverseMap();
        }

        private void LoadCategoryMapping()
        {
            CreateMap<CategoryRequestDto, CategoryEditVm>().ReverseMap();
            CreateMap<CategoryResponseDto, CategoryListVm>().ReverseMap();
        }

        private void LoadBrandMapping()
        {
            CreateMap<BrandRequestDto, BrandEditVm>().ReverseMap();
            CreateMap<BrandResponseDto, BrandListVm>().ReverseMap();
        }
    }
}
