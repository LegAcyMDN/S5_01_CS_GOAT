using System.Security.Cryptography;
using System.Text;

namespace S5_01_App_CS_GOAT.Services
{
    public class SecurityService
    {
        /// <summary>
        /// Create a new (non-cryptographically secure) random seed
        /// </summary>
        /// <returns>A random string</returns>
        public static string GenerateSeed(int lenght = 16)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, lenght)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Generates a new random token
        /// </summary>
        /// <returns>A Base64 encoded random token</returns>
        public static string GenerateToken(int lenght = 64)
        {
            byte[] bytes = new byte[lenght];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Hashes a password using PBKDF2 with the provided salt
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <param name="salt">The salt to use for hashing</param>
        /// <returns>A Base64 encoded hash of the password</returns>
        public static string HashAndSalt(string password, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt))
                throw new ArgumentException("Password or salt cannot be null or empty");

            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Verifies if the given password matches the stored hash
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hash">The stored hash to compare against</param>
        /// <param name="salt">The salt used for hashing</param>
        /// <returns>True if the password matches, false otherwise</returns>
        public static bool? VerifyPassword(string? password, string? hash, string? salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(salt))
                return null;
            string hashedInput = HashAndSalt(password, salt);
            return hashedInput == hash;
        }
    }
}