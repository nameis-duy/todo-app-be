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
        public static string GenerateToken(this Account account, IOptions<JwtSetting> setting)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(setting.Value.PrivateKey);
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GenerateClaims(account),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = credentials,
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private static ClaimsIdentity GenerateClaims(Account account)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Name, account.LastName));
            claims.AddClaim(new Claim(ClaimTypes.Email, account.Email));
            claims.AddClaim(new Claim("Id", account.Id.ToString()));
            return claims;
        }
    }
}
