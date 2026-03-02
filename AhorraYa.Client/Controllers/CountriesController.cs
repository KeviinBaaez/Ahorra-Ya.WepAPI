using AhorraYa.Application.Dtos.Country;
using AhorraYa.WebClient.ViewModels.Countries;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    public class CountriesController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7284/");
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly string _jwtToken;

        public CountriesController(IMapper mapper)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _mapper = mapper;
            //Una vez autorizado mediante la webAPI, establecer tu nuevo token aquí.
            _jwtToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjJlZDM4OTZjLWM2ZDUtNDUzYi1hMzE4LTA4ZGU1NDgwZmM5NyIsInN1YiI6IjJlZDM4OTZjLWM2ZDUtNDUzYi1hMzE4LTA4ZGU1NDgwZmM5NyIsIm5hbWUiOiJBZG1pbiIsImVtYWlsIjoiYWRtaW5AYWhvcnJheWEuY29tIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzcyNDY4OTQ1LCJleHAiOjE3NzI0ODMzNDUsImlhdCI6MTc3MjQ2ODk0NX0.1pObFuc-jE0tXr6ntKqvlUZkb1L1YHpiZR-krd0EaTWshOEak8JinEyd0bvq9Z1OwbmzhYzAcBSC46MSL87yHQ";
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderCountries)
        {
            List<CountryListVm> list = new List<CountryListVm>();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Countries/All?searchText={searchText}&orderBy={orderCountries}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<CountryListVm>>(data);
            }
            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentOrderCountries = orderCountries ?? "A-Z";

            return View(list);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (id is null || id == 0)
            {
                var model = new CountryEditVm()
                {
                    Id = 0
                };
                return View(model);
            }
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                int idToFetch = id.Value;
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Countries/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    CountryRequestDto? countryDto = JsonConvert.DeserializeObject<CountryRequestDto>(data);

                    if (countryDto is null)
                    {
                        return NotFound($"Country With Id {id} Not Found!!");
                    }

                    CountryEditVm countryVm = _mapper.Map<CountryEditVm>(countryDto);
                    return View(countryVm);
                }

                return NotFound($"Country With Id {id} Not Found. API status: {response.StatusCode}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CountryEditVm countryVm)
        {
            if (ModelState.IsValid)
            {
                CountryRequestDto countryRequest = _mapper.Map<CountryRequestDto>(countryVm);
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                    string jsonContent = JsonConvert.SerializeObject(countryRequest);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;
                    if (countryRequest.Id == 0)
                    {
                        response = await _httpClient.PostAsync("api/Countries/Create", content);
                        successMessage = "successfully created country";
                    }
                    else
                    {
                        string url = $"api/Countries/Update?id={countryRequest.Id}";
                        response = await _httpClient.PutAsync(url, content);
                        successMessage = "successfully update country";
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
            return View(countryVm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                int idToFetch = id.Value;
                HttpResponseMessage response;

                response = await _httpClient.GetAsync($"api/Countries/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    CountryRequestDto? countryDto = JsonConvert.DeserializeObject<CountryRequestDto>(data);

                    if (countryDto is null)
                    {
                        return NotFound($"Country With Id {id} Not Found!!");
                    }

                    CountryEditVm countryVm = _mapper.Map<CountryEditVm>(countryDto);
                    return View(countryVm);
                }
                else
                {
                    return NotFound($"Country With Id {id} Not Found. API status: {response.StatusCode}");
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Error while trying to get a country";
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
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                int idToDelete = id.Value;


                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Countries/Remove?id={idToDelete}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Country deleted correctly";
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

