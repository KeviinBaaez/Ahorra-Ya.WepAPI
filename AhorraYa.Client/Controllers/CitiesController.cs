using AhorraYa.Application.Dtos.City;
using AhorraYa.WebClient.Filters;
using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Cities;
using AhorraYa.WebClient.ViewModels.Countries;
using AhorraYa.WebClient.ViewModels.Provinces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    [AdminOnly]
    public class CitiesController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        private HttpClient? _httpClient;

        public CitiesController(IMapper mapper, ApiService apiService)
        {
            _apiService = apiService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderCities)
        {
            List<CityListVm>? list = new List<CityListVm>();
            _httpClient = _apiService.CreateClient();
            //Envió una petición al endpoint y guardo la rta completa del servidor
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Cities/All?searchText={searchText}&orderBy={orderCities}");

            if (response.IsSuccessStatusCode)//(200 y 299)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<CityListVm>>(data);
            }

            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentOrderCities = orderCities ?? "A-Z";

            return View(list);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            _httpClient = _apiService.CreateClient();

            HttpResponseMessage response;

            if (id is null || id == 0)
            {
                var model = new CityEditVm
                {
                    Id = 0,
                    Provinces = await GetProvincesSelectListAsync()
                };
                return View(model);
            }
            try
            {
                _httpClient = _apiService.CreateClient();
                int idToFetch = id.Value;

                response = await _httpClient.GetAsync($"api/Cities/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    CityRequestDto? cityRequestDto = JsonConvert.DeserializeObject<CityRequestDto>(data);

                    if (cityRequestDto is null)
                    {
                        return NotFound($"City With Id {id} Not Found!!");
                    }
                    CityEditVm cityEditVm = _mapper.Map<CityEditVm>(cityRequestDto);
                    cityEditVm.Provinces = await GetProvincesSelectListAsync();
                    return View(cityEditVm);
                }
                return NotFound($"City With Id {id} Not Found. API status: {response.StatusCode}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CityEditVm cityEditVm)
        {
            if (ModelState.IsValid)
            {
                CityRequestDto cityRequestDto = _mapper.Map<CityRequestDto>(cityEditVm);
                try
                {
                    _httpClient = _apiService.CreateClient();
                    var jsonContent = JsonConvert.SerializeObject(cityRequestDto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;

                    if (cityRequestDto.Id == 0)
                    {
                        response = await _httpClient.PostAsync($"api/Cities/Create", content);
                        successMessage = "Successfully created City";
                    }
                    else
                    {
                        string url = $"api/Cities/Update?id={cityRequestDto.Id}";
                        response = await _httpClient.PutAsync(url, content);
                        successMessage = "Successfully update City";
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
                        cityEditVm.Provinces = await GetProvincesSelectListAsync();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error {ex.Message}");
                    throw;
                }
            }
            return View(cityEditVm);
        }

        private async Task<IEnumerable<SelectListItem>> GetProvincesSelectListAsync()
        {
            _httpClient = _apiService.CreateClient();

            var response = await _httpClient.GetAsync("api/Provinces/All");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<SelectListItem>();

            var json = await response.Content.ReadAsStringAsync();

            var provinces = JsonConvert.DeserializeObject<List<ProvinceListVm>>(json);

            if (provinces == null || !provinces.Any())
                return Enumerable.Empty<SelectListItem>();

            return provinces.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.ProvinceName
            });
        }

        private async Task<IEnumerable<SelectListItem>> GetCountriesSelectListAsync()
        {
            _httpClient = _apiService.CreateClient();

            var response = await _httpClient.GetAsync($"api/Countries/All");

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<SelectListItem>();
            }
            var json = await response.Content.ReadAsStringAsync();

            var countries = JsonConvert.DeserializeObject<List<CountryListVm>>(json);

            if (countries == null || !countries.Any())
            {
                return Enumerable.Empty<SelectListItem>();
            }
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

                response = await _httpClient.GetAsync($"api/Cities/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    CityRequestDto? cityDto = JsonConvert.DeserializeObject<CityRequestDto>(data);

                    if (cityDto is null)
                    {
                        return NotFound($"City With Id {id} Not Found!!");
                    }

                    CityEditVm cityEditVm = _mapper.Map<CityEditVm>(cityDto);
                    return View(cityEditVm);
                }
                else
                {
                    return NotFound($"City With Id {id} Not Found. API status: {response.StatusCode}");
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Error while trying to get a city";
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


                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Cities/Remove?id={idToDelete}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "City deleted correctly";
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
