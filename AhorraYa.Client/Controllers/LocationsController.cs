using AhorraYa.Application.Dtos.Location;
using AhorraYa.WebClient.Filters;
using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Cities;
using AhorraYa.WebClient.ViewModels.Locations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    [AdminOnly]
    public class LocationsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        private HttpClient? _httpClient;

        public LocationsController(IMapper mapper, ApiService apiService)
        {
            _mapper = mapper;
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderBy)
        {
            List<LocationListVm>? list = new List<LocationListVm>();
            _httpClient = _apiService.CreateClient();
            //Envió una petición al endpoint y guardo la rta completa del servidor
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Locations/All?searchText={searchText}&orderBy={orderBy}");

            if (response.IsSuccessStatusCode)//(200 y 299)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<LocationListVm>>(data);
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
                var model = new LocationEditVm
                {
                    Id = 0,
                    Cities = await GetCitiesSelectListAsync()
                };
                return View(model);
            }
            try
            {
                _httpClient = _apiService.CreateClient();
                int idToFetch = id.Value;

                response = await _httpClient.GetAsync($"api/Locations/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    LocationRequestDto? locationRequestDto = JsonConvert.DeserializeObject<LocationRequestDto>(data);

                    if (locationRequestDto is null)
                    {
                        return NotFound($"Location With Id {id} Not Found!!");
                    }
                    LocationEditVm locationEditVm = _mapper.Map<LocationEditVm>(locationRequestDto);
                    locationEditVm.Cities = await GetCitiesSelectListAsync();
                    return View(locationEditVm);
                }
                return NotFound($"Location With Id {id} Not Found. API status: {response.StatusCode}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(LocationEditVm locationEditVm)
        {
            if (ModelState.IsValid)
            {
                LocationRequestDto locationRequestDto = _mapper.Map<LocationRequestDto>(locationEditVm);
                try
                {
                    _httpClient = _apiService.CreateClient();
                    var jsonContent = JsonConvert.SerializeObject(locationRequestDto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;

                    if (locationRequestDto.Id == 0)
                    {
                        response = await _httpClient.PostAsync($"api/Locations/Create", content);
                        successMessage = "Successfully created Location";
                    }
                    else
                    {
                        string url = $"api/Locations/Update?id={locationRequestDto.Id}";
                        response = await _httpClient.PutAsync(url, content);
                        successMessage = "Successfully update location";
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
                        locationEditVm.Cities = await GetCitiesSelectListAsync();

                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error {ex.Message}");
                    throw;
                }
            }
            return View(locationEditVm);
        }

        private async Task<IEnumerable<SelectListItem>> GetCitiesSelectListAsync()
        {
            _httpClient = _apiService.CreateClient();

            var response = await _httpClient.GetAsync("api/Cities/All");

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<SelectListItem>();
            }
            var json = await response.Content.ReadAsStringAsync();

            var cities = JsonConvert.DeserializeObject<List<CityListVm>>(json);

            if (cities is null || !cities.Any())
            {
                return Enumerable.Empty<SelectListItem>();
            }

            return cities.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CityName
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

                response = await _httpClient.GetAsync($"api/Locations/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    LocationRequestDto? locationDto = JsonConvert.DeserializeObject<LocationRequestDto>(data);

                    if (locationDto is null)
                    {
                        return NotFound($"Category With Id {id} Not Found!!");
                    }

                    LocationEditVm locationEditVm = _mapper.Map<LocationEditVm>(locationDto);
                    return View(locationEditVm);
                }
                else
                {
                    return NotFound($"Location With Id {id} Not Found. API status: {response.StatusCode}");
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Error while trying to get a location";
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


                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Locations/Remove?id={idToDelete}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Location deleted correctly";
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
