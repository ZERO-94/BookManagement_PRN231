using BookManagement.API.Models.Requests;
using BookManagement.Infrastructure.Models;
using BookManagement.Infrastructure.Repositories.BookRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.API.Controllers
{
    [ApiController]
    [Route("books")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public class BookController : ODataController
    {

        private readonly IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository) {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var booksQuery = _bookRepository.GetAll(includeFunc: query => query.Include(x => x.Press).Include(x => x.Location));
            return Ok(booksQuery);
        }

        [HttpGet("{key}")]
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_bookRepository.FirstOrDefault(expression: x => x.Id == key, includeFunc: query => query.Include(x => x.Press).Include(x => x.Location)));
        }

        [HttpPost]
        [EnableQuery]
        public IActionResult Post([FromBody] CreateBookRequest request)
        {
            Book book = new Book()
            {
                Author = request.Author,
                ISBN = request.ISBN,
                Location = new Infrastructure.Models.Address()
                {
                    City = request.Location.City,
                    Street = request.Location.Street
                },
                PressId = request.PressId,
                Title= request.Title,
                Price= request.Price,
                
            };
            _bookRepository.Add(book);
            return Ok();
        }

        [HttpDelete("{key}")]
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var book = _bookRepository.FirstOrDefault(expression: x => x.Id == key, includeFunc: query => query.Include(x => x.Press).Include(x => x.Location));
            if (book == null) return NotFound();

            _bookRepository.Remove(book);
            return Ok();
        }
    }
}
