using Application.Constant;
using Application.Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ClaimService(IHttpContextAccessor contextAccessor) : IClaimService
    {
        private readonly HttpContext? httpContext = contextAccessor.HttpContext;

        public string GetClaim(string claimType)
        {
            if (httpContext is not null)
            {
                var claim = httpContext.User.FindFirst(claimType)
                    ?? throw new KeyNotFoundException($"The claim type {claimType} does not exists");
                return claim.Value;
            }
            return string.Empty;
        }

        public int GetCurrentUserId()
        {
            if (httpContext is null) return -1;
            var isSucced = int.TryParse(httpContext.User.FindFirst(ClaimConstant.USER_ID)?.Value, out var userId);
            return isSucced ? userId : -1;
        }
    }
}
