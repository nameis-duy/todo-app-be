namespace Infrastructure.Security
{
    public class JwtSetting
    {
        public const string Section = "Jwt";

        public string Audience { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Key { get; set; } = null!;
        public int TokenExpirationMinutes { get; set; }
    }
}
