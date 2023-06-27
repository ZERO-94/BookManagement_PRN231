using BookManagement.Webapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookManagement.Webapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string endpoint = "https://localhost:7129";

        public HomeController(ILogger<HomeController> logger)
        {
            

            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel request)
        {
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage res = await httpClient.PostAsJsonAsync<LoginModel>(endpoint + "/auth/login", request);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var token = res.Content.ReadFromJsonAsync<LoginResponse>().Result.AccessToken;
                    Response.Cookies.Append("token", token);
                    return RedirectToAction("Index", "Book");
                }
                return View(request);
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginModel request)
        {
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage res = await httpClient.PostAsJsonAsync<LoginModel>(endpoint + "/auth/register", request);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return RedirectToAction("Login");
                }
                return View(request);
            }
        }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set;}
    }
}
