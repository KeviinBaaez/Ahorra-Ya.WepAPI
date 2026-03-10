using AhorraYa.WebClient.ViewModels.Login;
using AhorraYa.WebClient.ViewModels.Token;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace AhorraYa.WebClient.Controllers
{
    public class LoginsController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginsController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7284/");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserEditVm userEditVm)
        {
            if (!ModelState.IsValid)
            {
                return View(userEditVm);
            }

            var json = JsonConvert.SerializeObject(userEditVm);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Auth/Login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Credenciales no válidas.");
                return View(userEditVm);
            }

            var result = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponseVm>(result);

            HttpContext.Session.SetString("JWToken", token.Token);

            return RedirectToAction("Index", "Home");
        }
    }
}
