using BookManagement.API.Extensions;
using BookManagement.API.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginRequest request)
        {
            return Ok(new { accessToken = _jwtService.GenerateJSONWebToken(request.Username) });
        }
    }
}
