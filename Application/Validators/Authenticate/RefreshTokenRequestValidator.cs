using Application.Constant;
using Application.DTOs.Authenticate;
using Application.Interface.Service;
using FluentValidation;

namespace Application.Validators.Authenticate
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator(ICacheService cacheService)
        {
            RuleFor(r => r.CurrentRefreshToken)
                .CustomAsync(async (token, context, ct) =>
                {
                    var dto = context.InstanceToValidate;
                    var cachedToken = await cacheService.GetAsync<string>(string.Format(CacheConstant.REFRESH_TOKEN_CACHE_ID, dto.UserId));
                    if (cachedToken is null || cachedToken != token)
                    {
                        context.AddFailure("Invalid refresh token");
                        return;
                    }
                });
        }
    }
}
