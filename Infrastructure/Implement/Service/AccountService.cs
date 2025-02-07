using Application.Constant;
using Application.DTOs.Account;
using Application.DTOs.Authenticate;
using Application.DTOs.Base;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using Domain.Entity;
using Infrastructure.ExtensionService;
using Infrastructure.Security;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Implement.Service
{
    public class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IOptions<JwtSetting> jwtSetting;
        private readonly ITimeService timeService;
        private readonly ICacheService cacheService;
        private readonly IClaimService claimService;

        public AccountService(IGenericRepo<Account> entityRepo,
                              IUnitOfWork uow,
                              IOptions<JwtSetting> jwtSetting,
                              ITimeService timeService,
                              ICacheService cacheService,
                              IClaimService claimService) : base(entityRepo, uow)
        {
            this.jwtSetting = jwtSetting;
            this.timeService = timeService;
            this.cacheService = cacheService;
            this.claimService = claimService;
        }

        public async Task<ResponseResult<AuthenticateResult>> AuthenticateAsync(AuthenticateRequest dto)
        {
            var account = await entityRepo.GetAll().FirstOrDefaultAsync(a => a.Email.ToLower() == dto.Email.ToLower());
            if (account is not null)
            {
                var isValidPassword = dto.Password.VerifyPassword(account.PasswordHash);
                if (isValidPassword)
                {
                    var now = timeService.GetCurrentLocalDateTime();
                    var accessToken = account.GenerateToken(now, jwtSetting);
                    var refreshTokenMinutesValid = 60 * 24;//24 hours

                    var refreshTokenSecretKey = account.Email + "" + account.LastName;
                    var hashedRefreshTokenSecreyKey = refreshTokenSecretKey.ComputeSha256Hash();
                    var refreshToken = account.GenerateToken(now, jwtSetting,
                        hashedRefreshTokenSecreyKey,
                        minuteValid: refreshTokenMinutesValid);

                    //cache refresh token in distributed cache
                    await cacheService.SetAsync(string.Format(CacheConstant.REFRESH_TOKEN_CACHE_ID, account.Id),
                                                refreshToken,
                                                refreshTokenMinutesValid);

                    return new ResponseResult<AuthenticateResult>
                    {
                        Data = new AuthenticateResult
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken,
                        },
                        IsSucceed = true,
                        Message = "Login succeed"
                    };
                }
            }

            return new ResponseResult<AuthenticateResult>
            {
                Message = "Incorrect email or password",
                IsSucceed = false
            };
        }

        public async Task<ResponseResult<string>> ChangePasswordAsync(AccountChangePasswordRequest dto)
        {
            var currentUserId = claimService.GetCurrentUserId();
            var account = await entityRepo.FindAsync(currentUserId)
                ?? throw new UnauthorizedAccessException("You are not allowed to do this");
            var isValidPassword = dto.OldPassword.VerifyPassword(account.PasswordHash);
            if (isValidPassword)
            {
                var newPasswordHash = dto.NewPassword.Hash();
                account.PasswordHash = newPasswordHash;
                entityRepo.Update(account);
                if (await uow.SaveChangeAsync())
                {
                    return new ResponseResult<string>
                    {
                        Data = string.Empty,
                        IsSucceed = true
                    };
                }
                throw new DbUpdateException("Error while update account to db");
            }
            return new ResponseResult<string>
            {
                Data = string.Empty,
                IsSucceed = false,
                Message = "Password incorrect"
            };
        }

        public async Task<ResponseResult<AuthenticateResult>> RefreshTokenAsync(RefreshTokenRequest dto)
        {
            var cachedRefreshToken = await cacheService.GetAsync<string>(string.Format(CacheConstant.REFRESH_TOKEN_CACHE_ID, dto.UserId));
            if (cachedRefreshToken is not null)
            {
                if (cachedRefreshToken == dto.CurrentRefreshToken)
                {
                    var account = new Account
                    {
                        Id = dto.UserId,
                        Email = dto.Email,
                        LastName = dto.LastName,
                    };
                    var now = timeService.GetCurrentLocalDateTime();
                    var accessToken = account.GenerateToken(now, jwtSetting);
                    var refreshTokenMinutesValid = 60 * 24;//expired after 24 hours

                    var refreshTokenSecretKey = account.Email + "" + account.LastName;
                    var hashedRefreshTokenSecreyKey = refreshTokenSecretKey.ComputeSha256Hash();
                    var refreshToken = account.GenerateToken(now, jwtSetting,
                        hashedRefreshTokenSecreyKey,
                        minuteValid: refreshTokenMinutesValid);

                    //cache refresh token in distributed cache
                    await cacheService.SetAsync(string.Format(CacheConstant.REFRESH_TOKEN_CACHE_ID, account.Id),
                                                refreshToken,
                                                refreshTokenMinutesValid);

                    return new ResponseResult<AuthenticateResult>
                    {
                        Data = new AuthenticateResult
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken
                        },
                        IsSucceed = true,
                    };
                }
            }
            return new ResponseResult<AuthenticateResult>
            {
                IsSucceed = false,
                Message = "Refresh Token is invalid or does not exists"
            };
        }

        public async Task<ResponseResult<AccountVM>> RegisterAsync(RegisterRequest dto)
        {
            var account = dto.Adapt<Account>();
            account.PasswordHash = dto.Password.Hash();
            await entityRepo.AddAsync(account);
            if (await uow.SaveChangeAsync())
            {
                return new ResponseResult<AccountVM>
                {
                    Data = account.Adapt<AccountVM>(),
                    Message = "Succeed",
                    IsSucceed = true,
                };
            }
            throw new DbUpdateException("Error while create account to db");
        }

        public async Task<ResponseResult<AccountVM>> UpdateAccountAsync(AccountUpdateRequest dto)
        {
            var currentUserId = claimService.GetCurrentUserId();
            var account = await entityRepo.FindAsync(currentUserId) 
                ?? throw new UnauthorizedAccessException("You are not allowed to do this.");
            dto.Adapt(account);
            entityRepo.Update(account);
            if (await uow.SaveChangeAsync())
            {
                return new ResponseResult<AccountVM>
                {
                    Data = account.Adapt<AccountVM>(),
                    Message = "Succeed",
                    IsSucceed = true,
                };
            };
            throw new DbUpdateException("Error while update account to db");
        }
    }
}
