using AhorraYa.Application.Dtos.Category;
using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Categories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        private HttpClient? _httpClient;

        public CategoriesController(IMapper mapper, ApiService apiService)
        {
            _mapper = mapper;
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderCategories)
        {
            List<CategoryListVm> list = new List<CategoryListVm>();

            _httpClient = _apiService.CreateClient();
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Categories/All?searchText={searchText}&orderBy={orderCategories}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<CategoryListVm>>(data);
            }
            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentOrderCategories = orderCategories ?? "A-Z";

            return View(list);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (id is null || id == 0)
            {
                var model = new CategoryEditVm()
                {
                    Id = 0
                };
                return View(model);
            }
            try
            {
                _httpClient = _apiService.CreateClient();
                int idToFetch = id.Value;
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Categories/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    CategoryRequestDto? categoryDto = JsonConvert.DeserializeObject<CategoryRequestDto>(data);

                    if (categoryDto is null)
                    {
                        return NotFound($"Category With Id {id} Not Found!!");
                    }

                    CategoryEditVm categoryVm = _mapper.Map<CategoryEditVm>(categoryDto);
                    return View(categoryVm);
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
        public async Task<IActionResult> Upsert(CategoryEditVm categoryVm)
        {
            if (ModelState.IsValid)
            {
                CategoryRequestDto categoryRequest = _mapper.Map<CategoryRequestDto>(categoryVm);
                try
                {
                    _httpClient = _apiService.CreateClient();
                    string jsonContent = JsonConvert.SerializeObject(categoryRequest);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;
                    if (categoryRequest.Id == 0)
                    {
                        response = await _httpClient.PostAsync("api/Categories/Create", content);
                        successMessage = "successfully created category";
                    }
                    else
                    {
                        string url = $"api/Categories/Update?id={categoryRequest.Id}";
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
            return View(categoryVm);
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

                response = await _httpClient.GetAsync($"api/Categories/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    CategoryRequestDto? categoryDto = JsonConvert.DeserializeObject<CategoryRequestDto>(data);

                    if (categoryDto is null)
                    {
                        return NotFound($"Category With Id {id} Not Found!!");
                    }

                    CategoryEditVm categoryVm = _mapper.Map<CategoryEditVm>(categoryDto);
                    return View(categoryVm);
                }
                else
                {
                    return NotFound($"Category With Id {id} Not Found. API status: {response.StatusCode}");
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Error while trying to get a category";
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


                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Categories/Remove?id={idToDelete}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Category deleted correctly";
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
