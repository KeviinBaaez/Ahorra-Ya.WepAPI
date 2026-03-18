using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Brands;
using AhorraYa.WebClient.ViewModels.PricesOfShops;
using AhorraYa.WebClient.ViewModels.Shops;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace AhorraYa.WebClient.Controllers
{
    public class PricesOfShopsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        private HttpClient? _httpClient;

        public PricesOfShopsController(IMapper mapper, ApiService apiService)
        {
            _mapper = mapper;
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string orderBy, int? brandId, int? shopId)
        {
            _httpClient = _apiService.CreateClient();
            List<PriceOfShopListVm>? list = new List<PriceOfShopListVm>();
            //Envió una petición al endpoint y guardo la rta completa del servidor
            HttpResponseMessage response = await 
                _httpClient.GetAsync($"api/PriceOfShop/All?searchText={searchText}&orderBy={orderBy}&brandId={brandId}&shopId={shopId}");

            if (response.IsSuccessStatusCode)// (200 y 299)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<PriceOfShopListVm>>(data);
            }
            ViewBag.Brands = await GetBrandsSelectListAsync();
            ViewBag.Shops = await GetShopsSelectListAsync();

            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentOrderBy = orderBy ?? "A-Z";

            ViewBag.CurrentBrandId = brandId;
            ViewBag.CurrentShopId = shopId;

            return View(list);
        }

        private async Task<IEnumerable<SelectListItem>> GetBrandsSelectListAsync()
        {
            _httpClient = _apiService.CreateClient();

            var response = await _httpClient.GetAsync("api/Brands/All");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<SelectListItem>();

            var json = await response.Content.ReadAsStringAsync();

            var brands = JsonConvert.DeserializeObject<List<BrandListVm>>(json);

            if (brands == null || !brands.Any())
                return Enumerable.Empty<SelectListItem>();

            return brands.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.BrandName
            });
        }

        private async Task<IEnumerable<SelectListItem>> GetShopsSelectListAsync()
        {
            _httpClient = _apiService.CreateClient();

            var response = await _httpClient.GetAsync("api/Shops/All");
            if(!response.IsSuccessStatusCode)
                return Enumerable.Empty<SelectListItem>();

            var json = await response.Content.ReadAsStringAsync();

            var shops = JsonConvert.DeserializeObject<List<ShopListVm>>(json);

            if(shops == null || !shops.Any())
            {
                return Enumerable.Empty<SelectListItem>();
            }

            return shops.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.ShopName
            });
        }
    }
}
