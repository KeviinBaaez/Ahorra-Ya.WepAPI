using AhorraYa.Application.Dtos.Brand;
using AhorraYa.Application.Dtos.Category;
using AhorraYa.WebClient.ViewModels.Brand;
using AhorraYa.WebClient.ViewModels.Category;
using AutoMapper;

namespace AhorraYa.WebClient.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            LoadCategoryMapping();
            LoadBrandMapping();
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
