

namespace BookManagement.API.Models.Requests
{
    public class CreateBookRequest
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public Address Location { get; set; }
        public int PressId { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public string Street { get; set; }
    } 

}
