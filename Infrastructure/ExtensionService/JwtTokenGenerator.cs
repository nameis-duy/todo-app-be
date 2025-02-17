using Application.Interface.Service;
using Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.ExtensionService
{
    public class JwtTokenGenerator(IOptions<JwtSetting> tokenSetting, ITimeService timeService) : IJwtTokenGenerator
    {
        public string GenerateToken(int id, string name, string email, DateTime? now,
            string? secretKey = null, int? minuteValid = null)
        {
            var jwtSetting = tokenSetting.Value;
            now ??= timeService.GetCurrentLocalDateTime();
            var key = Encoding.UTF8.GetBytes(secretKey ?? jwtSetting.Key);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey,
                                                     SecurityAlgorithms.HmacSha256Signature);
            var claims = GenerateClaims(id, name, email);
            var token = new JwtSecurityToken(jwtSetting.Issuer,
                                             jwtSetting.Audience,
                                             claims,
                                             expires: now.Value.AddMinutes(minuteValid ?? jwtSetting.TokenExpirationMinutes),
                                             signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private IEnumerable<Claim> GenerateClaims(int id, string name, string email)
        {
            IEnumerable<Claim> claims = [
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim("Id", id.ToString())
                ];
            return claims;
        }
    }
}
