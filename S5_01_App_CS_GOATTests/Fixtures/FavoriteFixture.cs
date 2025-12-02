using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class FavoriteFixture
    {
        public static Favorite GetFavorite()
        {
            return new Favorite
            {
                UserId = 2,
                CaseId = 1
            };
        }

        public static Favorite GetOtherUserFavorite()
        {
            return new Favorite
            {
                UserId = 1,
                CaseId = 1
            };
        }

        public static (int, int) GetFavoriteKey(int userId, int caseId)
        {
            return (userId, caseId);
        }

        public static (int, int) GetFavoriteKey1ForNormalUser()
        {
            return (2, 1);
        }

        public static (int, int) GetFavoriteKey2ForNormalUser()
        {
            return (2, 2);
        }

        public static (int, int) GetFavoriteKey3ForNormalUser()
        {
            return (2, 3);
        }
    }
}
