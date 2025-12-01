using S5_01_App_CS_GOAT.Services;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class User : IUserDependant
    {
        public int? DependantUserId { get => this.UserId; }

        private static string GenerateRandomSeed()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Hashes a password using PBKDF2 with the provided salt
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>A Base64 encoded hash of the password</returns>
        private string HashAndSalt(string password)
        {
            if (string.IsNullOrEmpty(password)|| string.IsNullOrEmpty(SaltPassword))
                throw new ArgumentException("Password or salt cannot be null or empty");
            byte[] saltBytes = Convert.FromBase64String(SaltPassword);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Hashes the given password and sets the PasswordHash and PasswordSalt properties
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        private void hashpassword(string password) {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(saltBytes);
            this.SaltPassword = Convert.ToBase64String(saltBytes);
            this.HashPassword = HashAndSalt(password);
        }

        /// <summary>
        /// Verifies if the given password matches the stored PasswordHash
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <returns>True if the password matches, false otherwise</returns>
        private bool verifypassword(string password) {
            string hashedInput = HashAndSalt(password);
            return hashedInput == this.HashPassword;
        }
    }
}