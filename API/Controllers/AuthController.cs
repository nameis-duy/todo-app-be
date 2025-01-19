using Application.DTOs.Authenticate;
using Application.DTOs.Base;
using Application.Interface.Service;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AuthController(IAccountService accountService) : BaseController
    {
        [ApiVersion(1)]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromServices] IValidator<RegisterRequest> validator,
                                                       [FromBody] RegisterRequest dto)
        {
            var validateResult = await validator.ValidateAsync(dto);
            if (validateResult.IsValid is false)
            {
                var errors = validateResult.Errors.ConvertAll(err => new ResponseResult<string>
                {
                    Data = err.PropertyName,
                    Message = err.ErrorMessage,
                    IsSucceed = false
                });
                return BadRequest(errors);
            }
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
        [Authorize]
        public async Task<IActionResult> RefreshTokenAsync([FromServices] IValidator<RefreshTokenRequest> validator,
                                                           [FromBody] RefreshTokenRequest dto)
        {
            var validateResult = await validator.ValidateAsync(dto);
            if (validateResult.IsValid is false)
            {
                var errors = validateResult.Errors.ConvertAll(err => new ResponseResult<string>
                {
                    Data = err.PropertyName,
                    Message = err.ErrorMessage,
                    IsSucceed = false
                });
                return BadRequest(errors);
            }
            var result = await accountService.RefreshTokenAsync(dto);
            if (result.IsSucceed is true) return Ok(result.Data);
            return BadRequest(result.Message);
        }
    }
}
