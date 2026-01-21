using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class UserFixture
    {
        public static User GetAdminUser()
        {
            return new User
            {
                UserId = 1,
                Login = "admin",
                IsAdmin = true,
                Email = "admin@test.com",
                DisplayName = "Admin User"
            };
        }

        public static User GetNormalUser()
        {
            return new User
            {
                UserId = 2,
                Login = "user",
                IsAdmin = false,
                Email = "user@test.com",
                DisplayName = "Normal User"
            };
        }

        public static List<User> GetUsers()
        {
            return new List<User>
            {
                GetAdminUser(),
                GetNormalUser(),
                new User
                {
                    UserId = 3,
                    Login = "user3",
                    IsAdmin = false,
                    Email = "user3@test.com",
                    DisplayName = "Third User"
                }
            };
        }

        public static UserDTO GetAdminUserDTO()
        {
            return new UserDTO
            {
                UserId = 1,
                Login = "admin",
                IsAdmin = true,
                Email = "admin@test.com",
                DisplayName = "Admin User"
            };
        }

        public static UserDTO GetNormalUserDTO()
        {
            return new UserDTO
            {
                UserId = 2,
                Login = "user",
                IsAdmin = false,
                Email = "user@test.com",
                DisplayName = "Normal User"
            };
        }

        public static List<UserDTO> GetUserDTOs()
        {
            return new List<UserDTO>
            {
                GetAdminUserDTO(),
                GetNormalUserDTO()
            };
        }

        public static LoginDTO GetValidLoginDTO()
        {
            return new LoginDTO
            {
                Identifier = "user",
                Password = "password123",
                Remember = null
            };
        }

        public static LoginDTO GetInvalidLoginDTO()
        {
            return new LoginDTO
            {
                Identifier = "wronguser",
                Password = "wrongpassword",
                Remember = null
            };
        }

        public static TokenDTO GetRememberToken()
        {
            return new TokenDTO
            {
                TokenId = 1,
                TokenValue = "test-remember-token",
                TokenExpiry = DateTime.UtcNow.AddDays(30),
                UserId = 2
            };
        }

        public static AuthDTO GetAuthDTO()
        {
            return new AuthDTO
            {
                UserId = 2,
                DisplayName = "Normal User",
                JwtToken = "test-jwt-token-12345"
            };
        }
    }
}
