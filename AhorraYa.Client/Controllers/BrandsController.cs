using AhorraYa.Application.Dtos.Brand;
using AhorraYa.WebClient.Filters;
using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Brands;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    [AdminOnly]
    public class BrandsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        private HttpClient? _httpClient;
        public BrandsController(IMapper mapper, ApiService apiService)
        {
            _mapper = mapper;
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderBrands)
        {
            _httpClient = _apiService.CreateClient();
            List<BrandListVm>? list = new List<BrandListVm>();
            //Envió una petición al endpoint y guardo la rta completa del servidor
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Brands/All?searchText={searchText}&orderBy={orderBrands}");

            if (response.IsSuccessStatusCode)//(200 y 299)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<BrandListVm>>(data);
            }

            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentOrderBrands = orderBrands ?? "A-Z";

            return View(list);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (id is null || id == 0)
            {
                var model = new BrandEditVm()
                {
                    Id = 0
                };
                return View(model);
            }
            try
            {

                _httpClient = _apiService.CreateClient();
                int idToFetch = id.Value;

                HttpResponseMessage response = await _httpClient.GetAsync($"api/Brands/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    BrandRequestDto? brandRequestDto = JsonConvert.DeserializeObject<BrandRequestDto>(data);

                    if (brandRequestDto is null)
                    {
                        return NotFound($"Brand With Id {id} Not Found!!");
                    }

                    BrandEditVm brandVm = _mapper.Map<BrandEditVm>(brandRequestDto);
                    return View(brandVm);
                }
                return NotFound($"Brand With Id {id} Not Found. API status: {response.StatusCode}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Upsert(BrandEditVm brandVm)
        {
            if (ModelState.IsValid)
            {
                BrandRequestDto brandRequestDto = _mapper.Map<BrandRequestDto>(brandVm);
                try
                {
                    _httpClient = _apiService.CreateClient();
                    string jsonContent = JsonConvert.SerializeObject(brandRequestDto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;

                    if (brandRequestDto.Id == 0)
                    {
                        response = await _httpClient.PostAsync("api/Brands/Create", content);
                        successMessage = "successfully created brand";
                    }
                    else
                    {
                        string url = $"api/Brands/Update?id={brandRequestDto.Id}";
                        response = await _httpClient.PutAsync(url, content);
                        successMessage = "successfully update brand";
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = successMessage;
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        string errorData = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error {ex.Message}");
                }
            }
            return View(brandVm);
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
                int IdToFetch = id.Value;
                HttpResponseMessage response;

                response = await _httpClient.GetAsync($"api/Brands/GetById?id={IdToFetch}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    BrandRequestDto? brandRequestDto = JsonConvert.DeserializeObject<BrandRequestDto>(data);

                    if (brandRequestDto is null)
                    {
                        return NotFound($"Brand With Id {id} Not Found!!");
                    }
                    BrandEditVm brandVm = _mapper.Map<BrandEditVm>(brandRequestDto);
                    return View(brandVm);
                }
                else
                {
                    return NotFound($"Brand With Id {id} Not Found. API status: {response.StatusCode}");
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Error while trying to get a Brand";
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

                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Brands/Remove?id={idToDelete}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Brand deleted correctly";
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
