using AhorraYa.Application.Dtos.Province;
using AhorraYa.WebClient.Filters;
using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Countries;
using AhorraYa.WebClient.ViewModels.Provinces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    [AdminOnly]
    public class ProvincesController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        private HttpClient? _httpClient;

        public ProvincesController(IMapper mapper, ApiService apiService)
        {
            _mapper = mapper;
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderProvinces)
        {
            List<ProvinceListVm>? list = new List<ProvinceListVm>();
            _httpClient = _apiService.CreateClient();
            //Envió una petición al endpoint y guardo la rta completa del servidor
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Provinces/All?searchText={searchText}&orderBy={orderProvinces}");

            if (response.IsSuccessStatusCode)//(200 y 299)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<ProvinceListVm>>(data);
            }

            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentOrderProvinces = orderProvinces ?? "A-Z";

            return View(list);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            _httpClient = _apiService.CreateClient();

            HttpResponseMessage response;

            if (id is null || id == 0)
            {
                var model = new ProvinceEditVm
                {
                    Id = 0,
                    Countries = await GetCountriesSelectListAsync()
                };
                return View(model);
            }
            try
            {
                _httpClient = _apiService.CreateClient();
                int idToFetch = id.Value;

                response = await _httpClient.GetAsync($"api/Provinces/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    ProvinceRequestDto? provinceRequestDto = JsonConvert.DeserializeObject<ProvinceRequestDto>(data);

                    if (provinceRequestDto is null)
                    {
                        return NotFound($"Province With Id {id} Not Found!!");
                    }
                    ProvinceEditVm provinceEditVm = _mapper.Map<ProvinceEditVm>(provinceRequestDto);
                    provinceEditVm.Countries = await GetCountriesSelectListAsync();
                    return View(provinceEditVm);
                }
                return NotFound($"Province With Id {id} Not Found. API status: {response.StatusCode}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProvinceEditVm provinceEditVm)
        {
            if (ModelState.IsValid)
            {
                ProvinceRequestDto provinceRequestDto = _mapper.Map<ProvinceRequestDto>(provinceEditVm);
                try
                {
                    _httpClient = _apiService.CreateClient();
                    var jsonContent = JsonConvert.SerializeObject(provinceRequestDto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;

                    if (provinceRequestDto.Id == 0)
                    {
                        response = await _httpClient.PostAsync($"api/Provinces/Create", content);
                        successMessage = "Successfully created Province";
                    }
                    else
                    {
                        string url = $"api/Provinces/Update?id={provinceRequestDto.Id}";
                        response = await _httpClient.PutAsync(url, content);
                        successMessage = "Successfully update Province";
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
                        provinceEditVm.Countries = await GetCountriesSelectListAsync();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error {ex.Message}");
                    throw;
                }
            }
            return View(provinceEditVm);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountriesSelectListAsync()
        {
            _httpClient = _apiService.CreateClient();

            var response = await _httpClient.GetAsync("api/Countries/All");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<SelectListItem>();

            var json = await response.Content.ReadAsStringAsync();

            var countries = JsonConvert.DeserializeObject<List<CountryListVm>>(json);

            if (countries == null || !countries.Any())
                return Enumerable.Empty<SelectListItem>();

            return countries.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CountryName
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

                response = await _httpClient.GetAsync($"api/Provinces/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    ProvinceRequestDto? provinceDto = JsonConvert.DeserializeObject<ProvinceRequestDto>(data);

                    if (provinceDto is null)
                    {
                        return NotFound($"Province With Id {id} Not Found!!");
                    }

                    ProvinceEditVm provinceEditVm = _mapper.Map<ProvinceEditVm>(provinceDto);
                    return View(provinceEditVm);
                }
                else
                {
                    return NotFound($"Province With Id {id} Not Found. API status: {response.StatusCode}");
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Error while trying to get a province";
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


                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Provinces/Remove?id={idToDelete}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Province deleted correctly";
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