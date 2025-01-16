using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Security
{
    public class TokenValidationConfiguration(IOptions<JwtSetting> settings) : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly JwtSetting jwtSetting = settings.Value;

        public void Configure(string? name, JwtBearerOptions options) => Configure(options);

        public void Configure(JwtBearerOptions options)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.PrivateKey ?? throw new KeyNotFoundException("Jwt Screcet key not found."))),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSetting.Issuer,
                ValidAudience = jwtSetting.Audience
            };
        }
    }
}
