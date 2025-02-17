using Application.Constant;
using Application.DTOs.Account;
using Application.DTOs.Authenticate;
using Application.DTOs.Base;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using Domain.Entity;
using Mapster;

namespace Application.Services
{
    public class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IAccountRepo accountRepo;
        private readonly ITimeService timeService;
        private readonly ICacheService cacheService;
        private readonly IClaimService claimService;
        private readonly IJwtTokenGenerator jwtTokenGenerator;
        private readonly IPasswordHelper passwordHelpher;

        public AccountService(IAccountRepo accountRepo,
                              IUnitOfWork uow,
                              ITimeService timeService,
                              ICacheService cacheService,
                              IClaimService claimService,
                              IJwtTokenGenerator jwtTokenGenerator,
                              IPasswordHelper passwordService) : base(accountRepo, uow)
        {
            this.accountRepo = accountRepo;
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.timeService = timeService;
            this.cacheService = cacheService;
            this.claimService = claimService;
            this.passwordHelpher = passwordService;
        }

        public async Task<ResponseResult<AuthenticateResult>> AuthenticateAsync(AuthenticateRequest dto)
        {
            var account = await accountRepo.FirstOrDefaultAsync(a => a.Email.ToLower() == dto.Email.ToLower());
            if (account is not null)
            {
                var isValidPassword = passwordHelpher.VerifyPassword(dto.Password, account.PasswordHash);
                if (isValidPassword)
                {
                    var now = timeService.GetCurrentLocalDateTime();
                    var accessToken = jwtTokenGenerator.GenerateToken(account.Id, account.LastName, account.Email, now);
                    var refreshTokenMinutesValid = 60 * 24;//24 hours

                    var refreshTokenSecretKey = account.Email + "" + account.LastName;
                    var hashedRefreshTokenSecreyKey = passwordHelpher.ComputeSha256Hash(refreshTokenSecretKey);
                    var refreshToken = jwtTokenGenerator.GenerateToken(account.Id, account.LastName, account.Email, now,
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
            var account = await accountRepo.FirstOrDefaultAsync(acc => acc.Id == currentUserId)
                ?? throw new UnauthorizedAccessException("You are not allowed to do this");
            var isValidPassword = passwordHelpher.VerifyPassword(dto.OldPassword, account.PasswordHash);
            if (isValidPassword)
            {
                var newPasswordHash = passwordHelpher.Hash(dto.NewPassword);
                account.PasswordHash = newPasswordHash;
                accountRepo.Update(account);
                if (await uow.SaveChangeAsync())
                {
                    return new ResponseResult<string>
                    {
                        Data = string.Empty,
                        IsSucceed = true
                    };
                }
                throw new SystemException("Error while update account to db");
            }
            return new ResponseResult<string>
            {
                Data = string.Empty,
                IsSucceed = false,
                Message = "Password incorrect"
            };
        }

        public async Task<ResponseResult<AccountVM>> GetAccountInformationAsync()
        {
            var currentUserId = claimService.GetCurrentUserId();
            var account = await accountRepo.FirstOrDefaultAsync(acc => acc.Id == currentUserId)
                ?? throw new UnauthorizedAccessException("You are not allowed to do this");
            return new ResponseResult<AccountVM>
            {
                Data = account.Adapt<AccountVM>(),
                IsSucceed = true
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
                    var accessToken = jwtTokenGenerator.GenerateToken(account.Id, account.LastName, account.Email, now);
                    var refreshTokenMinutesValid = 60 * 24;//expired after 24 hours

                    var refreshTokenSecretKey = account.Email + "" + account.LastName;
                    var hashedRefreshTokenSecreyKey = passwordHelpher.ComputeSha256Hash(refreshTokenSecretKey);
                    var refreshToken = jwtTokenGenerator.GenerateToken(account.Id, account.LastName, account.Email, now,
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
            account.PasswordHash = passwordHelpher.Hash(dto.Password);
            await accountRepo.AddAsync(account);
            if (await uow.SaveChangeAsync())
            {
                return new ResponseResult<AccountVM>
                {
                    Data = account.Adapt<AccountVM>(),
                    Message = "Succeed",
                    IsSucceed = true,
                };
            }
            throw new SystemException("Error while create account to db");
        }

        public async Task<ResponseResult<AccountVM>> UpdateAccountAsync(AccountUpdateRequest dto)
        {
            var currentUserId = claimService.GetCurrentUserId();
            var account = await accountRepo.FirstOrDefaultAsync(acc => acc.Id == currentUserId)
                ?? throw new UnauthorizedAccessException("You are not allowed to do this.");
            dto.Adapt(account);
            accountRepo.Update(account);
            if (await uow.SaveChangeAsync())
            {
                return new ResponseResult<AccountVM>
                {
                    Data = account.Adapt<AccountVM>(),
                    Message = "Succeed",
                    IsSucceed = true,
                };
            };
            throw new SystemException("Error while update account to db");
        }
    }
}
