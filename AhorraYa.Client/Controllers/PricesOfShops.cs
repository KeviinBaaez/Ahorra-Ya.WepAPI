using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.PricesOfShops;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AhorraYa.WebClient.Controllers
{
    public class PricesOfShops : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        private HttpClient? _httpClient;

        public PricesOfShops(IMapper mapper, ApiService apiService)
        {
            _mapper = mapper;
            _apiService = apiService;
        }
        [HttpGet]
        private async Task<IActionResult> Index(string? searchText, string orderBy)
        {
            _httpClient = _apiService.CreateClient();
            List<PriceOfShopListVm>? list = new List<PriceOfShopListVm>();
            //Envió una petición al endpoint y guardo la rta completa del servidor
            HttpResponseMessage response = await 
                _httpClient.GetAsync($"api/PriceOfShop/GetAll?searchText={searchText}&orderBy={orderBy}");

            if (response.IsSuccessStatusCode)// (200 y 299)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<PriceOfShopListVm>>(data);
            }

            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentOrderBrands = orderBy ?? "A-Z";

            return View(list);
        }
    }
}
