using AhorraYa.WebClient.Services;
using AhorraYa.WebClient.ViewModels.Login;
using AhorraYa.WebClient.ViewModels.Token;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AhorraYa.WebClient.Controllers
{
    public class LoginsController : Controller
    {
        private readonly ApiService _apiService;
        private HttpClient? _httpClient;

        public LoginsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterEditMv registerEdit)
        {
            try
            {
                Console.WriteLine("=== INICIO REGISTER ===");

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState inválido");
                    return View(registerEdit);
                }

                if (registerEdit.Password != registerEdit.ConfirmPassword)
                {
                    Console.WriteLine("Las contraseñas no coinciden");

                    ModelState.AddModelError("", "Las contraseñas no coinciden");
                    return View(registerEdit);
                }

                var json = JsonConvert.SerializeObject(new
                {
                    name = registerEdit.Name,
                    userName = registerEdit.UserName,
                    email = registerEdit.Email,
                    password = registerEdit.Password
                });

                Console.WriteLine($"Registrando usuario: {registerEdit.UserName}");
                Console.WriteLine($"Email: {registerEdit.Email}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient = _apiService.CreateClient();

                Console.WriteLine($"URL API: {_httpClient.BaseAddress}");

                var response = await _httpClient.PostAsync("api/Auth/Register", content);

                Console.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("Error API:");
                    Console.WriteLine(error);

                    ModelState.AddModelError("", error);
                    return View(registerEdit);
                }

                Console.WriteLine("Usuario registrado correctamente");

                return RedirectToAction("VerifyCode", new
                {
                    username = registerEdit.UserName
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR EN REGISTER");
                Console.WriteLine(ex.ToString());

                ModelState.AddModelError("", ex.Message);
                return View(registerEdit);
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(string username, string code)
        {
            try
            {
                Console.WriteLine("=== INICIO VERIFY CODE ===");
                Console.WriteLine($"Username: {username}");
                Console.WriteLine($"Code: {code}");

                var json = JsonConvert.SerializeObject(new
                {
                    Username = username,
                    Code = code
                });

                _httpClient = _apiService.CreateClient();

                Console.WriteLine($"URL API: {_httpClient.BaseAddress}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/Auth/VerifyCode", content);

                Console.WriteLine($"StatusCode: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Cuenta verificada correctamente");

                    TempData["SuccessMessage"] = "¡Cuenta activada con éxito! Ya podés iniciar sesión.";

                    return RedirectToAction("Login");
                }

                var apiError = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Error API:");
                Console.WriteLine(apiError);

                ModelState.AddModelError("", "Código incorrecto o vencido.");
                ViewBag.UserName = username;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR EN VERIFY CODE");
                Console.WriteLine(ex.ToString());

                ModelState.AddModelError("", ex.Message);
                ViewBag.UserName = username;

                return View();
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterEditMv registerEdit)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(registerEdit);
        //    }
        //    if (registerEdit.Password != registerEdit.ConfirmPassword)
        //    {
        //        ModelState.AddModelError("", "Las contraseñas no coinciden");
        //        return View(registerEdit);
        //    }
        //    var json = JsonConvert.SerializeObject(new
        //    {
        //        name = registerEdit.Name,
        //        userName = registerEdit.UserName,
        //        email = registerEdit.Email,
        //        password = registerEdit.Password
        //    });

        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    _httpClient = _apiService.CreateClient();

        //    var response = await _httpClient.PostAsync("api/Auth/Register", content);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var error = await response.Content.ReadAsStringAsync();

        //        ModelState.AddModelError("", error);

        //        return View(registerEdit);
        //    }

        //    return RedirectToAction("VerifyCode", new { username = registerEdit.UserName });
        //}

        //[HttpGet]
        //public IActionResult VerifyCode(string username)
        //{
        //    // Le pasás el email a la vista para saber a qué usuario activar
        //    ViewBag.Username = username;
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> VerifyCode(string username, string code)
        //{
        //    // Armás el objeto para pegarle al endpoint de verificación que creamos en la API
        //    var json = JsonConvert.SerializeObject(new
        //    {
        //        Username = username,
        //        Code = code
        //    });

        //    _httpClient = _apiService.CreateClient();

        //    var content = new StringContent(json, Encoding.UTF8, "application/json");
        //    var response = await _httpClient.PostAsync("api/Auth/VerifyCode", content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        TempData["SuccessMessage"] = "¡Cuenta activada con éxito! Ya podés iniciar sesión.";
        //        return RedirectToAction("Login");
        //    }
        //    var apiError = await response.Content.ReadAsStringAsync();
        //    System.Diagnostics.Debug.WriteLine($"Error de la API: {apiError}");

        //    ModelState.AddModelError("", "Código incorrecto o vencido.");
        //    ViewBag.UserName = username;
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Login(UserEditVm userEditVm)
        {
            if (!ModelState.IsValid)
            {
                return View(userEditVm);
            }
            try
            {
                _httpClient = _apiService.CreateClient();

                var json = JsonConvert.SerializeObject(userEditVm);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Auth/Login", content);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return Content($"ERROR API: {response.StatusCode}\n\n{result}");
                }

                var token = JsonConvert.DeserializeObject<TokenResponseVm>(result);

                HttpContext.Session.SetString("JWToken", token.Token);
                HttpContext.Session.SetString("UserName", token.UserName);
                HttpContext.Session.SetString("Role", token.Role);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }

            //var json = JsonConvert.SerializeObject(userEditVm);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            //var response = await _httpClient.PostAsync("api/Auth/Login", content);

            //if (!response.IsSuccessStatusCode)
            //{
            //    ModelState.AddModelError("", "Credenciales no válidas.");
            //    return View(userEditVm);
            //}

            //var result = await response.Content.ReadAsStringAsync();
            //var token = JsonConvert.DeserializeObject<TokenResponseVm>(result);

            //HttpContext.Session.SetString("JWToken", token.Token);
            //HttpContext.Session.SetString("UserName", token.UserName);
            //HttpContext.Session.SetString("Role", token.Role);

            //return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Logins");
        }
    }
}
