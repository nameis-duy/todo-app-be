namespace Application.Interface.Service
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(int id, string name, string email, DateTime? now,
            string? secretKey = null, int? minuteValid = null);
    }
}
