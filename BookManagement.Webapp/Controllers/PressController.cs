using BookManagement.Infrastructure.Models;
using BookManagement.Webapp.Extensions;
using BookManagement.Webapp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace BookManagement.Webapp.Controllers
{
    [Authorize]
    public class PressController : Controller
    {
        private string endpoint = "https://localhost:7129/odata";

        // GET: PressController
        public async Task<ActionResult> Index(int page = 1, string search = "")
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

                var response = await httpClient.GetAsync(endpoint + $"/presses?$count=true&$skip={(page - 1) * 5}&$top=5&$filter=contains(Name,'{search}')");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ODataResponse<List<PressModel>>>(json);
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

        // GET: PressController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<PressModel>(endpoint + $"/presses/{id}");

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
                var res = await httpClient.GetFromJsonAsync<PressModel>(endpoint + $"/presses/{id}");

                var viewModel = new Press()
                {
                    Id = id,
                    Name = res.Name,
                    
                };
                if(res.Category == Category.Book.GetDescription())
                {
                    viewModel.Category = Category.Book;
                } else if (res.Category == Category.EBook.GetDescription())
                {
                    viewModel.Category = Category.EBook;
                } else
                {
                    viewModel.Category = Category.Magazine;
                }


                ViewBag.Categories = new List<SelectListItem>() {
                new SelectListItem()
                {
                    Text=Category.Book.GetDescription(),
                    Value=((int)Category.Book).ToString(),
                    Selected=res.Category == Category.Book.GetDescription()
                },
                new SelectListItem()
                {
                    Text=Category.EBook.GetDescription(),
                    Value=((int)Category.EBook).ToString(),
                    Selected=res.Category == Category.EBook.GetDescription()
                },
                new SelectListItem()
                {
                    Text=Category.Magazine.GetDescription(),
                    Value=((int)Category.Magazine).ToString(),
                    Selected=res.Category == Category.Magazine.GetDescription()
                }
                };

                return View(viewModel);
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
                var res = await httpClient.GetFromJsonAsync<PressModel>(endpoint + $"/presses/{id}");

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
