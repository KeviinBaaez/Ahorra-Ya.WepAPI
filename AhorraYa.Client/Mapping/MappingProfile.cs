using AhorraYa.Application.Dtos.Brand;
using AhorraYa.Application.Dtos.Category;
using AhorraYa.Application.Dtos.Product;
using AhorraYa.WebClient.ViewModels.Brands;
using AhorraYa.WebClient.ViewModels.Categories;
using AhorraYa.WebClient.ViewModels.Product;
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
