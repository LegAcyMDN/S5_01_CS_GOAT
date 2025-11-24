using System;
using System.Linq;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class User : IUserDependant
    {
        public int? DependantUserId { get => this.UserId; }

        /// <summary>
        /// Generates a random 16-character string for the Seed property
        /// </summary>
        /// <returns>A random alphanumeric string of 16 characters</returns>
        private static string GenerateRandomSeed()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}