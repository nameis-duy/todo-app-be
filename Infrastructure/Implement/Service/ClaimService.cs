using Application.Constant;
using Application.Interface.Service;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Implement.Service
{
    public class ClaimService(IHttpContextAccessor contextAccessor) : IClaimService
    {
        private readonly HttpContext? httpContext = contextAccessor.HttpContext;

        public string GetClaim(string claimType)
        {
            if (httpContext is not null) return httpContext.User.FindFirstValue(claimType)
                    ?? throw new KeyNotFoundException($"The claim type {claimType} does not exists");
            return string.Empty;
        }

        public int GetCurrentUserId()
        {
            if (httpContext is null) return -1;
            var isSucced = int.TryParse(httpContext.User.FindFirstValue(ClaimConstant.USER_ID), out var userId);
            return isSucced ? userId : -1;
        }
    }
}
