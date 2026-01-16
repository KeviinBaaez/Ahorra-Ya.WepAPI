using AhorraYa.Application.Dtos.Brand;
using AhorraYa.Application.Dtos.Product;
using AhorraYa.WebClient.ViewModels.Brands;
using AhorraYa.WebClient.ViewModels.Categories;
using AhorraYa.WebClient.ViewModels.Product;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    public class ProductsController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7284/");
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly string _jwtToken;

        public ProductsController(IMapper mapper)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _mapper = mapper;
            //Una vez autorizado mediante la webAPI, establecer tu nuevo token aquí.
            _jwtToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjJlZDM4OTZjLWM2ZDUtNDUzYi1hMzE4LTA4ZGU1NDgwZmM5NyIsInN1YiI6IjJlZDM4OTZjLWM2ZDUtNDUzYi1hMzE4LTA4ZGU1NDgwZmM5NyIsIm5hbWUiOiJBZG1pbiIsImVtYWlsIjoiYWRtaW5AYWhvcnJheWEuY29tIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzY4NTI0MTEzLCJleHAiOjE3Njg1Mzg1MTMsImlhdCI6MTc2ODUyNDExM30.uLKewq1qRyMKA-18IfeXYOfUZu7MilKdzufguUPlZ_Ric0-wP02eXUbFEogmkYvY26LHAZf0wGg_gLXIRlck3Q";
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderProducts)
        {
            List<ProductListVm>? list = new List<ProductListVm>();
            //Paso el token de autorización.
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            //Envió una petición al endpoint y guardo la rta completa del servidor
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Products/All?searchText={searchText}&orderBy={orderProducts}");

            if (response.IsSuccessStatusCode)//(200 y 299)
            {
                string data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<ProductListVm>>(data);
            }

            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentOrderProducts = orderProducts ?? "A-Z";

            return View(list);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            HttpResponseMessage response;

            if (id is null || id == 0)
            {
                var model = new ProductEditVm
                {
                    Id = 0,
                    Categories = await GetCategoriesSelectListAsync(),
                    Brands = await GetBrandsSelectListAsync()
                };
                return View(model);
            }
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                int idToFetch = id.Value;

                response = await _httpClient.GetAsync($"api/Products/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    ProductRequestDto? productRequestDto = JsonConvert.DeserializeObject<ProductRequestDto>(data);

                    if (productRequestDto is null)
                    {
                        return NotFound($"Product With Id {id} Not Found!!");
                    }
                    ProductEditVm productEditVm = _mapper.Map<ProductEditVm>(productRequestDto);
                    productEditVm.Categories = await GetCategoriesSelectListAsync();
                    productEditVm.Brands = await GetBrandsSelectListAsync();
                    return View(productEditVm);
                }
                return NotFound($"Product With Id {id} Not Found. API status: {response.StatusCode}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductEditVm productEditVm)
        {
            if(ModelState.IsValid)
            {
                ProductRequestDto productRequestDto = _mapper.Map<ProductRequestDto>(productEditVm);
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                    var jsonContent = JsonConvert.SerializeObject(productRequestDto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;

                    if(productRequestDto.Id == 0)
                    {
                        response = await _httpClient.PostAsync($"api/Products/Create", content);
                        successMessage = "Successfully created Product";
                    }
                    else
                    {
                        string url = $"api/Products/Update?id={productRequestDto.Id}";
                        response = await _httpClient.PutAsync(url , content);
                        successMessage = "Successfully update Product";
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
                        productEditVm.Brands = await GetBrandsSelectListAsync();
                        productEditVm.Categories = await GetCategoriesSelectListAsync();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error {ex.Message}");
                    throw;
                }
            }
            return View(productEditVm);
        }

        private async Task<IEnumerable<SelectListItem>> GetBrandsSelectListAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _jwtToken);

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

        private async Task<IEnumerable<SelectListItem>> GetCategoriesSelectListAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _jwtToken);

            var response = await _httpClient.GetAsync($"api/Categories/All");

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<SelectListItem>();
            }
            var json = await response.Content.ReadAsStringAsync();

            var categories = JsonConvert.DeserializeObject<List<CategoryListVm>>(json);

            if (categories == null || !categories.Any())
            {
                return Enumerable.Empty<SelectListItem>();
            }
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName
            });
        }
    }
}
