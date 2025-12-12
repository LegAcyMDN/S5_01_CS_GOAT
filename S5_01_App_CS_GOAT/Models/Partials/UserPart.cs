using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.DTO.Helpers;
using S5_01_App_CS_GOAT.Services;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class User : IUserDependant
    {
        public int? DependantUserId { get => this.UserId; }

        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (password.Length < 8 || password.Length > 64) return false;
            int number = 0, upper = 0, lower = 0, special = 0;
            foreach (char c in password)
            {
                if (char.IsDigit(c)) number++;
                else if (char.IsUpper(c)) upper++;
                else if (char.IsLower(c)) lower++;
                else special++;
            }
            if (number < 1 || upper < 1 || lower < 1 || special < 1) return false;
            return true;
        }

        public bool TrySetPassword(string password) {
            if (!IsValidPassword(password)) return false;
            string newSalt = SecurityService.GenerateToken();
            string newHash = SecurityService.HashAndSalt(password, newSalt);
            this.SaltPassword = newSalt;
            this.HashPassword = newHash;
            return true;
        }
    }
}