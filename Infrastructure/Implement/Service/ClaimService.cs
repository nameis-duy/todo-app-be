using Application.Interface.Service;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Implement.Service
{
    public class ClaimService(IHttpContextAccessor httpContextAccessor) : IClaimService
    {
        public string GetClaim(string claimType)
        {
            var context = httpContextAccessor.HttpContext;
            if (context is not null) return context.User.FindFirstValue(claimType)
                    ?? throw new KeyNotFoundException($"The claim type {claimType} does not exists");
            return string.Empty;
        }
    }
}
