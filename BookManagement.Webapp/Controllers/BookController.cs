﻿using BookManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net;
using BookManagement.Webapp.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace BookManagement.Webapp.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private string endpoint = "https://localhost:7129";
        // GET: BookController
        public async Task<ActionResult> Index()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<List<Book>>(endpoint + "/books");

                return View(res);
            }
        }

        // GET: BookController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var res = await httpClient.GetFromJsonAsync<Book>(endpoint + $"/books/{id}");

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
                var res = await httpClient.GetFromJsonAsync<List<Press>>(endpoint + $"/presses");

                ViewBag.Presses = new SelectList(res?.ToList() ?? new List<Press>(), nameof(Press.Id), nameof(Press.Name));
                return View();
            }
        }

        // POST: BookController/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Book press)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                    var res = await httpClient.PostAsJsonAsync<Book>(endpoint + "/books", press);

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
                var res = await httpClient.GetFromJsonAsync<Book>(endpoint + $"/books/{id}");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                var resPresses = await httpClient.GetFromJsonAsync<List<Press>>(endpoint + $"/presses");

                ViewBag.Presses = new SelectList(resPresses?.ToList() ?? new List<Press>(), nameof(Press.Id), nameof(Press.Name), res.PressId);
                return View(res);
            };
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Book press)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
                    var res = await httpClient.PutAsJsonAsync<Book>(endpoint + $"/books/{id}", press);

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
                var res = await httpClient.GetFromJsonAsync<Book>(endpoint + $"/books/{id}");

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
