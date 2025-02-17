using BCrypt.Net;

namespace Application.Interface.Service
{
    public interface IPasswordHelper
    {
        string Hash(string password, HashType type = HashType.SHA512);
        bool VerifyPassword(string password, string passwordHash, HashType type = HashType.SHA512);
        string ComputeSha256Hash(string rawData);
    }
}
