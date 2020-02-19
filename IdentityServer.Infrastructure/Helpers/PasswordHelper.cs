using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IdentityServer.Infrastructure.Helpers
{
    public static class PasswordHelper
    {
        public static string GenerateSalt()
        {
            var randomBytes = new byte[128 / 8];

            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public static string HashPassword(string salt, string password)
        {
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            var hashed = KeyDerivation.Pbkdf2(
                password, Encoding.UTF8.GetBytes(salt), KeyDerivationPrf.HMACSHA512,
                10000, 256 / 8
            );
            return Convert.ToBase64String(hashed);
        }

        public static bool ValidateHashPassword(string salt, string password, string hashed)
        {
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(hashed))
                throw new ArgumentNullException(nameof(hashed));
            return HashPassword(salt, password) == hashed;
        }
    }
}