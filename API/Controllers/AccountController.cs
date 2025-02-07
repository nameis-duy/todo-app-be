using Application.DTOs.Account;
using Application.DTOs.Base;
using Application.Interface.Service;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController(IAccountService accountService) : BaseController
    {
        [ApiVersion(1)]
        [HttpGet("detail")]
        [Authorize]
        public async Task<IActionResult> GetAccountInformationAsync()
        {
            var account = await accountService.GetAccountInformationAsync();
            return Ok(account);
        }

        [ApiVersion(1)]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAccountAsync([FromServices] IValidator<AccountUpdateRequest> validator,
                                                      AccountUpdateRequest dto)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (validationResult.IsValid is false)
            {
                var errors = validationResult.Errors.ConvertAll(err => new ResponseResult<string>
                {
                    Data = err.PropertyName,
                    Message = err.ErrorMessage,
                    IsSucceed = false
                });
                return BadRequest(errors);
            }
            var result = await accountService.UpdateAccountAsync(dto);
            if (result.IsSucceed is true) return Ok(result);
            return BadRequest(result);
        }

        [ApiVersion(1)]
        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromServices] IValidator<AccountChangePasswordRequest> validator,
                                                             AccountChangePasswordRequest dto)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (validationResult.IsValid is false)
            {
                var errors = validationResult.Errors.ConvertAll(err => new ResponseResult<string>
                {
                    Data = err.PropertyName,
                    Message = err.ErrorMessage,
                    IsSucceed = false
                });
                return BadRequest(errors);
            }
            var result = await accountService.ChangePasswordAsync(dto);
            if (result.IsSucceed is true) return Ok(result);
            return BadRequest(result);
        }
    }
}
