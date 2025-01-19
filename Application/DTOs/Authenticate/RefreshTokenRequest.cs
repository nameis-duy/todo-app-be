namespace Application.DTOs.Authenticate
{
#pragma warning disable CS8618
    public class RefreshTokenRequest
    {
        public int UserId { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CurrentRefreshToken { get; set; }
    }
}
