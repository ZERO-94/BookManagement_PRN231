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
    
    public class BookController : ODataController
    {

        private readonly IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository) {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [EnableQuery]
        public IActionResult Get()
        {
            var booksQuery = _bookRepository.GetAll(includeFunc: query => query.Include(x => x.Press).Include(x => x.Location));
            return Ok(booksQuery);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableQuery]
        public IActionResult Get(int id)
        {
            return Ok(_bookRepository.FirstOrDefault(expression: x => x.Id == id, includeFunc: query => query.Include(x => x.Press).Include(x => x.Location)));
        }

        [HttpPost]
        [EnableQuery]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] BookRequest request)
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

        [HttpPut("{id}")]
        [EnableQuery]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(int id, [FromBody] BookRequest request)
        {
            var book = _bookRepository.FirstOrDefault(expression: x => x.Id == id, includeFunc: x => x.Include(book => book.Location));

            if(book == null)
            {
                return BadRequest();
            }

            book.Author = request.Author;
            book.ISBN = request.ISBN;
            book.Location.City = request.Location.City;
            book.Location.Street = request.Location.Street;
            book.PressId = request.PressId;
            book.Title = request.Title;
            book.Price = request.Price;

            _bookRepository.Update(book);
            return Ok();
        }

        [HttpDelete("{key}")]
        [EnableQuery]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int key)
        {
            var book = _bookRepository.FirstOrDefault(expression: x => x.Id == key, includeFunc: query => query.Include(x => x.Location));
            if (book == null) return BadRequest();

            _bookRepository.Remove(book);
            return Ok();
        }
    }
}
