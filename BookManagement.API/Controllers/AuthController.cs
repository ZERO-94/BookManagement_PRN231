using BookManagement.API.Extensions;
using BookManagement.API.Models.Requests;
using BookManagement.Infrastructure.Models;
using BookManagement.Infrastructure.Repositories.AccountRepository;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly JwtService _jwtService;
        private readonly IAccountRepository _accountRepository;

        public AuthController(JwtService jwtService, IAccountRepository accountRepository)
        {
            _jwtService = jwtService;
            _accountRepository = accountRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginRequest request)
        {

            var account = _accountRepository.FirstOrDefault(expression: x => x.Username == request.Username && x.Password == request.Password);

            if(account == null)
            {
                return Unauthorized();
            }

            return Ok(new { accessToken = _jwtService.GenerateJSONWebToken(account) });
        }


        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody] LoginRequest request)
        {

            var account = _accountRepository.FirstOrDefault(expression: x => x.Username == request.Username);

            if (account != null)
            {
                return BadRequest();
            }

            Account newAccount = new Account()
            {
                Password = request.Password,
                Username = request.Username,
                Role = Role.User
            };

            _accountRepository.Add(newAccount);

            return Ok();
        }
    }
}
