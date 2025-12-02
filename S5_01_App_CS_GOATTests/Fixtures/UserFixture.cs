using S5_01_App_CS_GOAT.Models.EntityFramework;

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
                IsAdmin = true
            };
        }

        public static User GetNormalUser()
        {
            return new User
            {
                UserId = 2,
                Login = "user",
                IsAdmin = false
            };
        }
    }
}
