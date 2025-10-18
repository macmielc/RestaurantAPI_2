using Microsoft.AspNetCore.Mvc;
using RestaurantAPI_2.Models;
using RestaurantAPI_2.Services;

namespace RestaurantAPI_2.Controllers
{
    [Route("api/account")]
    [ApiController] // Walidowanie wprowadzanych danych 
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService) 
        {
            _accountService = accountService;
        }
        
        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto userDto)
        {
            _accountService.RegisterUser(userDto);

            return Ok();

        }

        [HttpPost("login")]
        public ActionResult LoginUser([FromBody] LoginDto dto)
        {
            string token = _accountService.GenerateJwtToken(dto);

            return Ok(token);
        }

    }
}
