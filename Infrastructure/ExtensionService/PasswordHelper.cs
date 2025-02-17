using Application.Interface.Service;
using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.ExtensionService
{
    public class PasswordHelper : IPasswordHelper
    {
        public string Hash(string password, HashType type = HashType.SHA512)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, type);
        }

        public bool VerifyPassword(string password, string passwordHash, HashType type = HashType.SHA512)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash, type);
        }

        public string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
