using AhorraYa.Application.Dtos.Brand;
using AhorraYa.WebClient.ViewModels.Brand;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    public class BrandsController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7284/");
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly string _jwtToken;

        public BrandsController(IMapper mapper)
        {
            _mapper = mapper;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            //Una vez autorizado mediante la webAPI, establecer tu nuevo token aquí.
            _jwtToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJJZCI6ImE0NmM1ZmY3LTJkNDktNDNiMS0wZDM1LTA4ZGUyMjJkMzI2MyIsInN1YiI6ImE0NmM1ZmY3LTJkNDktNDNiMS0wZDM1LTA4ZGUyMjJkMzI2MyIsIm5hbWUiOiJhZG1pbiIsImVtYWlsIjoiYWRtaW5AYWRtaW4uY29tIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY2NTE0NDM5LCJleHAiOjE3NjY1Mjg4MzksImlhdCI6MTc2NjUxNDQzOX0.8s3sSblUlQ8_Yq_AnocV-4zwOuwUhwl_YkLR58NTezKSsTFrJh0hFklE9tOhuiZjqDpUZedxuH4ZZRxWD5fMqA";
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderBrands)
        {
            List<BrandListVm>? list = new List<BrandListVm>();
            //Paso el token de autorización.
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            //Envió una petición al endpoint y guardo la rta completa del servidor
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Brands/All?searchText={searchText}&orderBy={orderBrands}");

            if(response.IsSuccessStatusCode)//(200 y 299)
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
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
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
                return NotFound($"Category With Id {id} Not Found. API status: {response.StatusCode}");
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
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                    string jsonContent = JsonConvert.SerializeObject(brandRequestDto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;

                    if (brandRequestDto.Id == 0)
                    {
                        response = await _httpClient.PostAsync("api/Brands/Create", content);
                        successMessage = "successfully created Brand";
                    }
                    else
                    {
                        string url = $"api/Brands/Update?id={brandRequestDto.Id}";
                        response = await _httpClient.PutAsync(url, content);
                        successMessage = "successfully update category";
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
            if(id is null || id == 0)
            {
                return NotFound();
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                int IdToFetch = id.Value;
                HttpResponseMessage response;

                response = await _httpClient.GetAsync($"api/Brands/GetById?id={IdToFetch}");

                if(response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    BrandRequestDto? brandRequestDto = JsonConvert.DeserializeObject<BrandRequestDto>(data);

                    if(brandRequestDto is null)
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
            if(id is null || id == 0) 
            { 
                return NotFound(); 
            }
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                int idToDelete = id.Value;

                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Brands/Remove?id={idToDelete}");
                if(response.IsSuccessStatusCode)
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
