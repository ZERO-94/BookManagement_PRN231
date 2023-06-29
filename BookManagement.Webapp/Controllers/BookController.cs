using BookManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net;
using BookManagement.Webapp.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BookManagement.Webapp.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookManagement.Webapp.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private string endpoint = "https://localhost:7129/odata";
        // GET: BookController
        public async Task<ActionResult> Index(int page = 1, string search = "")
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                
                var response = await httpClient.GetAsync(endpoint + $"/books?$count=true&$expand=Location,Press&$skip={(page - 1)*5}&$top=5&$filter=contains(Title,'{search}')");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ODataResponse<List<BookModel>>>(json);
                    if (res != null)
                    {
                        ViewBag.TotalPage = Math.Ceiling((decimal)(res.Count ?? 1) / 5);
                        ViewBag.Page = page;
                        ViewBag.Search = search;
                        return View(res.Value);
                    }
                }

                return View();
            }
        }

        // GET: BookController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<BookModel>(endpoint + $"/books/{id}?$expand=Press,Location");

                return View(res);
            }
        }

        // GET: BookController/Create
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var response = await httpClient.GetAsync(endpoint + $"/presses");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ODataResponse<List<Press>>>(json);
                    if (res != null)
                    {
                        ViewBag.Presses = new SelectList(res?.Value.ToList() ?? new List<Press>(), nameof(Press.Id), nameof(Press.Name));
                        return View();
                    }
                }

                return View();
            }
        }

        // POST: BookController/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookModel press)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                    var res = await httpClient.PostAsJsonAsync<BookModel>(endpoint + "/books", press);

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

        // GET: BookController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<BookModel>(endpoint + $"/books/{id}?$expand=Press,Location");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var response = await httpClient.GetAsync(endpoint + $"/presses");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var resPresses = JsonConvert.DeserializeObject<ODataResponse<List<Press>>>(json);
                    if (resPresses != null)
                    {
                        ViewBag.Presses = new SelectList(resPresses?.Value.ToList() ?? new List<Press>(), nameof(Press.Id), nameof(Press.Name));
                        return View(res);
                    }
                }
                return View(res);
            };
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, BookModel press)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                    var res = await httpClient.PutAsJsonAsync<BookModel>(endpoint + $"/books/{id}", press);

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

        // GET: BookController/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<BookModel>(endpoint + $"/books/{id}?$expand=Location,Press");

                return View(res);
            }
        }

        // POST: BookController/Delete/5
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
                    var res = await httpClient.DeleteAsync(endpoint + $"/books/{id}");

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
