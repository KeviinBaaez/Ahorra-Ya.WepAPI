using AhorraYa.Application.Dtos.Shop;
using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Locations;
using AhorraYa.WebClient.ViewModels.Shops;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    public class ShopsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        private HttpClient? _httpClient;

        public ShopsController(IMapper mapper, ApiService apiService)
        {
            _mapper = mapper;
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderBy)
        {
            List<ShopListVm>? list = new List<ShopListVm>();
            //Paso el token de autorización.
            _httpClient = _apiService.CreateClient();
            //Envió una petición al endpoint y guardo la rta completa del servidor
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Shops/All?searchText={searchText}&orderBy={orderBy}");

            if (response.IsSuccessStatusCode)//(200 y 299)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<ShopListVm>>(data);
            }

            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentOrderBy = orderBy ?? "A-Z";

            return View(list);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            _httpClient = _apiService.CreateClient();

            HttpResponseMessage response;

            if (id is null || id == 0)
            {
                var model = new ShopEditVm
                {
                    Id = 0,
                    Locations = await GetLocationsSelectListAsync()
                };
                return View(model);
            }
            try
            {
                _httpClient = _apiService.CreateClient();
                int idToFetch = id.Value;

                response = await _httpClient.GetAsync($"api/Shops/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    ShopRequestDto? ShopRequestDto = JsonConvert.DeserializeObject<ShopRequestDto>(data);

                    if (ShopRequestDto is null)
                    {
                        return NotFound($"Shop With Id {id} Not Found!!");
                    }
                    ShopEditVm ShopEditVm = _mapper.Map<ShopEditVm>(ShopRequestDto);
                    ShopEditVm.Locations = await GetLocationsSelectListAsync();
                    return View(ShopEditVm);
                }
                return NotFound($"Shop With Id {id} Not Found. API status: {response.StatusCode}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ShopEditVm ShopEditVm)
        {
            if (ModelState.IsValid)
            {
                ShopRequestDto ShopRequestDto = _mapper.Map<ShopRequestDto>(ShopEditVm);
                try
                {
                    _httpClient = _apiService.CreateClient();
                    var jsonContent = JsonConvert.SerializeObject(ShopRequestDto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;

                    if (ShopRequestDto.Id == 0)
                    {
                        response = await _httpClient.PostAsync($"api/Shops/Create", content);
                        successMessage = "Successfully created Shop";
                    }
                    else
                    {
                        string url = $"api/Shops/Update?id={ShopRequestDto.Id}";
                        response = await _httpClient.PutAsync(url, content);
                        successMessage = "Successfully update Shop";
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
                        ShopEditVm.Locations = await GetLocationsSelectListAsync();

                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error {ex.Message}");
                    throw;
                }
            }
            return View(ShopEditVm);
        }

        private async Task<IEnumerable<SelectListItem>> GetLocationsSelectListAsync()
        {
            _httpClient = _apiService.CreateClient();

            var response = await _httpClient.GetAsync("api/Locations/All");

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<SelectListItem>();
            }
            var json = await response.Content.ReadAsStringAsync();

            var locations = JsonConvert.DeserializeObject<List<LocationListVm>>(json);

            if (locations is null || !locations.Any())
            {
                return Enumerable.Empty<SelectListItem>();
            }

            return locations.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.Address} {l.Number}, Floor: {l.Floor ?? 0}"
            });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            try
            {
                _httpClient = _apiService.CreateClient();
                int idToFetch = id.Value;
                HttpResponseMessage response;

                response = await _httpClient.GetAsync($"api/Shops/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    ShopRequestDto? ShopDto = JsonConvert.DeserializeObject<ShopRequestDto>(data);

                    if (ShopDto is null)
                    {
                        return NotFound($"Category With Id {id} Not Found!!");
                    }

                    ShopEditVm ShopEditVm = _mapper.Map<ShopEditVm>(ShopDto);
                    return View(ShopEditVm);
                }
                else
                {
                    return NotFound($"Shop With Id {id} Not Found. API status: {response.StatusCode}");
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Error while trying to get a Shop";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            try
            {
                _httpClient = _apiService.CreateClient();
                int idToDelete = id.Value;


                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Shops/Remove?id={idToDelete}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Shop deleted correctly";
                    return RedirectToAction("Index");
                }
                else
                {
                    string errorData = await response.Content.ReadAsStringAsync();
                    TempData["error"] = errorData;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                TempData["error"] = ex.Message;
                return RedirectToAction("Index");
            }

        }
    }
}
