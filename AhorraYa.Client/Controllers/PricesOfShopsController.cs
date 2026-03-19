using AhorraYa.Application.Dtos.PriceOfShop;
using AhorraYa.Entities;
using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Brands;
using AhorraYa.WebClient.ViewModels.PricesOfShops;
using AhorraYa.WebClient.ViewModels.Product;
using AhorraYa.WebClient.ViewModels.Shops;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

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

        public async Task<IActionResult> Upsert(int? id)
        {
            _httpClient = _apiService.CreateClient();

            HttpResponseMessage response;

            if(id is null || id == 0)
            {
                var model = new PriceOfShopEditVm
                {
                    Id = 0,
                    ProductLists = await GetProductsSelectListAsync(),
                    Shops = await GetShopsSelectListAsync(),
                };
                return View(model);
            }
            try
            {
                _httpClient = _apiService.CreateClient();
                int idToFetch = id.Value;

                response = await _httpClient.GetAsync($"api/PricesOfShops/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    PriceOfShopEditVm? priceOfShopEditVm = JsonConvert.DeserializeObject<PriceOfShopEditVm>(data);

                    if(priceOfShopEditVm is null)
                    {
                        return NotFound($"Product With Id {id} Not Found!!");
                    }
                    PriceOfShopEditVm updatePrice = _mapper.Map<PriceOfShopEditVm>(priceOfShopEditVm);
                    return View(updatePrice);
                }
                return NotFound($"Product With Id {id} Not Found. API status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(PriceOfShopEditVm priceOfShopEditVm)
        {
            if(ModelState.IsValid)
            {
                PriceOfShopRequestDto priceOfShopRequestDto = _mapper.Map<PriceOfShopRequestDto>(priceOfShopEditVm);
                try
                {
                    _httpClient = _apiService.CreateClient();
                    var json = JsonConvert.SerializeObject(priceOfShopRequestDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;
                    
                    if(priceOfShopRequestDto.Id == 0)
                    {
                        response = await _httpClient.PostAsync($"api/PriceOfShop/Create", content);
                        successMessage = "Successfully created PriceOfProduct";
                    }
                    else
                    {
                        UpdatePriceOfShopRequestDto updatePrice = _mapper.Map<UpdatePriceOfShopRequestDto>(priceOfShopRequestDto);
                        response = await _httpClient.PutAsync($"api/PriceOfShop/Update?id={updatePrice.Id}", content);
                        successMessage = "Successfully update Price";
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = successMessage;
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        string errorData = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error {errorData}");
                        priceOfShopEditVm.ProductLists = await GetProductsSelectListAsync();
                        priceOfShopEditVm.Shops = await GetShopsSelectListAsync();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error {ex.Message}");
                    throw;
                }

            }
            return View(priceOfShopEditVm);

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

        private async Task<List<ProductListVm>> GetProductsSelectListAsync()
        {
            _httpClient = _apiService.CreateClient();

            var response = await _httpClient.GetAsync("api/Products/All");
            if (!response.IsSuccessStatusCode)
            {
                return new List<ProductListVm>();
            }

            var json = await response.Content.ReadAsStringAsync();

            var products = JsonConvert.DeserializeObject<List<ProductListVm>>(json);

            if(products == null || !products.Any()) 
            {
                return new List<ProductListVm>();
            }

            return products;
        }
    }
}
