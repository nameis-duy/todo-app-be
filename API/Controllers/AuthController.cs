using Application.DTOs.Authenticate;
using Application.Interface.Service;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AuthController(IAccountService accountService) : BaseController
    {
        [ApiVersion(1)]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest dto)
        {
            var result = await accountService.RegisterAsync(dto);
            if (result.IsSucceed is true) return Created(string.Empty, result);
            return BadRequest();
        }

        [ApiVersion(1)]
        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateRequest dto)
        {
            var result = await accountService.AuthenticateAsync(dto);
            if (result.IsSucceed is true) return Ok(result.Data);
            return BadRequest(result.Message);
        }
        
        [ApiVersion(1)]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest dto)
        {
            var result = await accountService.RefreshTokenAsync(dto);
            if (result.IsSucceed is true) return Ok(result.Data);
            return BadRequest(result.Message);
        }
    }
}
