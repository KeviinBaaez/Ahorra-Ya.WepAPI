using AhorraYa.Application.Dtos.Brand;
using AhorraYa.Application.Dtos.Category;
using AhorraYa.Application.Dtos.City;
using AhorraYa.Application.Dtos.Country;
using AhorraYa.Application.Dtos.Location;
using AhorraYa.Application.Dtos.PriceOfShop;
using AhorraYa.Application.Dtos.Product;
using AhorraYa.Application.Dtos.Province;
using AhorraYa.Application.Dtos.Shop;
using AhorraYa.WebClient.ViewModels.Brands;
using AhorraYa.WebClient.ViewModels.Categories;
using AhorraYa.WebClient.ViewModels.Cities;
using AhorraYa.WebClient.ViewModels.Countries;
using AhorraYa.WebClient.ViewModels.Locations;
using AhorraYa.WebClient.ViewModels.PricesOfShops;
using AhorraYa.WebClient.ViewModels.Product;
using AhorraYa.WebClient.ViewModels.Provinces;
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
            LoadCountryMapping();
            LoadProvinceMapping();
            LoadCityMapping();
            LoadPriceOfShopMapping();
        }

        private void LoadPriceOfShopMapping()
        {
            CreateMap<PriceOfShopRequestDto, PriceOfShopEditVm>().ReverseMap();
            CreateMap<PriceOfShopResponseDto, PriceOfShopListVm>().ReverseMap();

        }

        private void LoadCityMapping()
        {
            CreateMap<CityRequestDto, CityEditVm>().ReverseMap();
            CreateMap<CityResponseDto, CityListVm>().ReverseMap();
        }

        private void LoadProvinceMapping()
        {
            CreateMap<ProvinceRequestDto, ProvinceEditVm>().ReverseMap();
            CreateMap<ProvinceResponseDto, ProvinceListVm>().ReverseMap();
        }

        private void LoadCountryMapping()
        {
            CreateMap<CountryRequestDto, CountryEditVm>().ReverseMap();
            CreateMap<CountryResponseDto, CountryListVm>().ReverseMap();
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
