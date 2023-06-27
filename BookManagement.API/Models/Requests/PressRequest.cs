using BookManagement.Infrastructure.Models;

namespace BookManagement.API.Models.Requests
{
    public class PressRequest
    {
        public string Name { get; set; }
        public Category Category { get; set; }
    }
}
