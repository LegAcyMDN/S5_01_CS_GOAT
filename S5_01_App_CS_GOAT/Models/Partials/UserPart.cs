using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    /// <summary>
    /// Partial class providing password validation and hashing methods for User entities
    /// </summary>
    public partial class User : IUserDependant
    {
        public int? DependantUserId => UserId;

        /// <summary>
        /// Validates if a password meets complexity requirements
        /// </summary>
        /// <remarks>
        /// Password must meet all of these criteria:
        /// - Length: between 8 and 64 characters
        /// - At least 1 digit
        /// - At least 1 uppercase letter
        /// - At least 1 lowercase letter
        /// - At least 1 special character
        /// </remarks>
        /// <param name="password">The password to validate</param>
        /// <returns>True if password is valid; false otherwise</returns>
        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (password.Length < 8 || password.Length > 64)
            {
                return false;
            }

            int number = 0, upper = 0, lower = 0, special = 0;
            foreach (char c in password)
            {
                if (char.IsDigit(c))
                {
                    number++;
                }
                else if (char.IsUpper(c))
                {
                    upper++;
                }
                else if (char.IsLower(c))
                {
                    lower++;
                }
                else
                {
                    special++;
                }
            }
            return number >= 1 && upper >= 1 && lower >= 1 && special >= 1;
        }

        /// <summary>
        /// Sets a new password for the user by generating a salt and hashing the password
        /// </summary>
        /// <param name="password">The new password to set</param>
        /// <param name="bypassValidity">If true, skips validation checks; use only in special cases</param>
        /// <returns>True if password was successfully set; false if password doesn't meet requirements</returns>
        public bool TrySetPassword(string password, bool bypassValidity = false)
        {
            if (!bypassValidity && !IsValidPassword(password))
            {
                return false;
            }

            string newSalt = SecurityService.GenerateToken();
            string newHash = SecurityService.HashAndSalt(password, newSalt);
            SaltPassword = newSalt;
            HashPassword = newHash;
            return true;
        }
    }
}