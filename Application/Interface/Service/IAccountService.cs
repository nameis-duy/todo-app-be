using Application.DTOs.Account;
using Application.DTOs.Authenticate;
using Application.DTOs.Base;
using Domain.Entity;

namespace Application.Interface.Service
{
    public interface IAccountService : IBaseService<Account>
    {
        Task<ResponseResult<AccountVM>> RegisterAsync(RegisterRequest dto);
        Task<ResponseResult<AuthenticateResult>> AuthenticateAsync(AuthenticateRequest dto);
        Task<ResponseResult<AuthenticateResult>> RefreshTokenAsync(RefreshTokenRequest dto);
        Task<ResponseResult<AccountVM>> UpdateAccountAsync(AccountUpdateRequest dto);
        Task<ResponseResult<string>> ChangePasswordAsync(AccountChangePasswordRequest dto);
    }
}
