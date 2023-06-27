using BookManagement.API.Models.Requests;
using BookManagement.Infrastructure.Models;
using BookManagement.Infrastructure.Repositories.PressRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.API.Controllers
{
    [ApiController]
    [Route("presses")]
    public class PressController : ODataController
    {
        private readonly IPressRepository _pressRepository;

        public PressController(IPressRepository pressRepository)
        {
            _pressRepository = pressRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [EnableQuery]
        public IActionResult Get()
        {
            var booksQuery = _pressRepository.GetAll();
            return Ok(booksQuery);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableQuery]
        public IActionResult Get(int id)
        {
            return Ok(_pressRepository.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost]
        [EnableQuery]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] PressRequest request)
        {
            Press press = new Press()
            {
                Category = request.Category,
                Name = request.Name
            };
            _pressRepository.Add(press);
            return Ok(press);
        }

        [HttpPut("{id}")]
        [EnableQuery]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(int id, [FromBody] PressRequest request)
        {
            var press = _pressRepository.FirstOrDefault(expression: x => x.Id == id);

            if (press == null) return BadRequest();

            press.Name = request.Name;
            press.Category = request.Category;

            _pressRepository.Update(press);
            return Ok(press);
        }

        [HttpDelete("{key}")]
        [EnableQuery]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int key)
        {
            var book = _pressRepository.FirstOrDefault(expression: x => x.Id == key);
            if (book == null) return BadRequest();

            _pressRepository.Remove(book);
            return Ok();
        }
    }
}
