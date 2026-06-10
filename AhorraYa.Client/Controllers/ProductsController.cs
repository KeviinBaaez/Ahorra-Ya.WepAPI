using AhorraYa.Application.Dtos.Product;
using AhorraYa.WebClient.Filters;
using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Brands;
using AhorraYa.WebClient.ViewModels.Categories;
using AhorraYa.WebClient.ViewModels.Product;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    [AdminOnly]
    public class ProductsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        private HttpClient? _httpClient;

        private readonly IWebHostEnvironment _environment;

        public ProductsController(IMapper mapper, ApiService apiService, IWebHostEnvironment environment)
        {
            _mapper = mapper;
            _apiService = apiService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchText, string? orderProducts)
        {
            List<ProductListVm>? list = new List<ProductListVm>();
            _httpClient = _apiService.CreateClient();
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
            _httpClient = _apiService.CreateClient();

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
                _httpClient = _apiService.CreateClient();
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductEditVm productEditVm)
        {
            if (ModelState.IsValid)
            {
                if(productEditVm.ImageFile != null && productEditVm.ImageFile.Length > 0)
                {
                    string folder = Path.Combine(_environment.WebRootPath, "Images/Products");

                    if(!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    //eliminar img anterior
                    if (!string.IsNullOrEmpty(productEditVm.Image))
                    {
                        string oldPath = Path.Combine(_environment.WebRootPath, productEditVm.Image.TrimStart('/'));

                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(productEditVm.ImageFile.FileName);

                    string filePath = Path.Combine(folder, fileName);
                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await productEditVm.ImageFile.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error saving image: " + ex.Message);
                    }

                    productEditVm.Image = "/Images/Products/" + fileName;
                }
                ProductRequestDto productRequestDto = _mapper.Map<ProductRequestDto>(productEditVm);
                try
                {
                    _httpClient = _apiService.CreateClient();
                    var jsonContent = JsonConvert.SerializeObject(productRequestDto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;
                    string successMessage;

                    if (productRequestDto.Id == 0)
                    {
                        response = await _httpClient.PostAsync($"api/Products/Create", content);
                        successMessage = "Successfully created Product";
                    }
                    else
                    {
                        string url = $"api/Products/Update?id={productRequestDto.Id}";
                        response = await _httpClient.PutAsync(url, content);
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
                }
            }
            return View(productEditVm);
        }

        private async Task<IEnumerable<SelectListItem>> GetBrandsSelectListAsync()
        {
            _httpClient = _apiService.CreateClient();

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
            _httpClient = _apiService.CreateClient();

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

                response = await _httpClient.GetAsync($"api/Products/GetById?id={idToFetch}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    ProductRequestDto? productDto = JsonConvert.DeserializeObject<ProductRequestDto>(data);

                    if (productDto is null)
                    {
                        return NotFound($"Product With Id {id} Not Found!!");
                    }

                    ProductEditVm productEditVm = _mapper.Map<ProductEditVm>(productDto);
                    return View(productEditVm);
                }
                else
                {
                    return NotFound($"Product With Id {id} Not Found. API status: {response.StatusCode}");
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Error while trying to get a product";
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


                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Products/Remove?id={idToDelete}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Product deleted correctly";
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
