using Domain.Entity;
using Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.ExtensionService
{
    public static class JwtUtils
    {
        public static string GenerateToken(this Account account,
            DateTime now,
                                           IOptions<JwtSetting> setting,
                                           string? secretKey = null,
                                           int? minuteValid = null)
        {
            var jwtSetting = setting.Value;
            var key = Encoding.UTF8.GetBytes(secretKey ?? jwtSetting.Key);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey,
                                                     SecurityAlgorithms.HmacSha256Signature);
            var claims = GenerateClaims(account);
            var token = new JwtSecurityToken(jwtSetting.Issuer,
                                             jwtSetting.Audience,
                                             claims,
                                             expires: now.AddMinutes(minuteValid ?? jwtSetting.TokenExpirationMinutes),
                                             signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static IEnumerable<Claim> GenerateClaims(Account account)
        {
            IEnumerable<Claim> claims = [
                new Claim(ClaimTypes.Name, account.LastName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim("Id", account.Id.ToString())
                ];
            return claims;
        }
    }
}
