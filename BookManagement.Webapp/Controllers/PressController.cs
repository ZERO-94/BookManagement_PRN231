using BookManagement.Infrastructure.Models;
using BookManagement.Webapp.Extensions;
using BookManagement.Webapp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Http.Headers;

namespace BookManagement.Webapp.Controllers
{
    [Authorize]
    public class PressController : Controller
    {
        private string endpoint = "https://localhost:7129";

        // GET: PressController
        public async Task<ActionResult> Index()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<List<Press>>(endpoint + "/presses");
                
                return View(res);
            }
        }

        // GET: PressController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<Press>(endpoint + $"/presses/{id}");

                return View(res);
            }
        }

        // GET: PressController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Categories = new List<SelectListItem>() {
                new SelectListItem()
                {
                    Text=Category.Book.GetDescription(),
                    Value=((int)Category.Book).ToString()
                },
                new SelectListItem()
                {
                    Text=Category.EBook.GetDescription(),
                    Value=((int)Category.EBook).ToString()
                },
                new SelectListItem()
                {
                    Text=Category.Magazine.GetDescription(),
                    Value=((int)Category.Magazine).ToString()
                }
            }; ;

            return View();
        }

        // POST: PressController/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Press press)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                    var res = await httpClient.PostAsJsonAsync<Press>(endpoint + "/presses", press);

                    if (res.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    return View(press);
                }

                
            }
            catch
            {
                return View(press);
            }
        }

        // GET: PressController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<Press>(endpoint + $"/presses/{id}");

                ViewBag.Categories = new List<SelectListItem>() {
                new SelectListItem()
                {
                    Text=Category.Book.GetDescription(),
                    Value=((int)Category.Book).ToString(),
                    Selected=res.Category == Category.Book
                },
                new SelectListItem()
                {
                    Text=Category.EBook.GetDescription(),
                    Value=((int)Category.EBook).ToString(),
                    Selected=res.Category == Category.EBook
                },
                new SelectListItem()
                {
                    Text=Category.Magazine.GetDescription(),
                    Value=((int)Category.Magazine).ToString(),
                    Selected=res.Category == Category.Magazine
                }
            };

                return View(res);
            }
        }

        // POST: PressController/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Press press)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                    var res = await httpClient.PutAsJsonAsync<Press>(endpoint + $"/presses/{id}", press);

                    if(res.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    return View(press);
                }
            }
            catch
            {
                return View(press);
            }
        }

        // GET: PressController/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<Press>(endpoint + $"/presses/{id}");

                return View(res);
            }
        }

        // POST: PressController/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                    var res = await httpClient.DeleteAsync(endpoint + $"/presses/{id}");

                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
