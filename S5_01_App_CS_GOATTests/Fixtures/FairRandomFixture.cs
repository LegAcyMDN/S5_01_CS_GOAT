using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class FairRandomFixture
    {
        public static List<FairRandomDTO> GetFairRandomDTOs()
        {
            return new List<FairRandomDTO>
            {
                new FairRandomDTO
                {
                    ServerSeed = "abc123seed456",
                    ServerHash = "hash123abc456def",
                    UserNonce = 1,
                    CombinedHash = "combined123hash456",
                    Fraction1 = 0.12345,
                    Fraction2 = 0.12345,
                    TransactionId = 1
                },
                new FairRandomDTO
                {
                    ServerSeed = "xyz789seed012",
                    ServerHash = "hash789xyz012ghi",
                    UserNonce = 2,
                    CombinedHash = "combined789hash012",
                    Fraction1 = 0.67890,
                    Fraction2 = 0.12345,
                    TransactionId = 2
                },
                new FairRandomDTO
                {
                    ServerSeed = "def456seed789",
                    ServerHash = "hash456def789jkl",
                    UserNonce = 3,
                    CombinedHash = "combined456hash789",
                    Fraction1 = 0.98765
                }
            };
        }

        public static FairRandomDTO GetSingleFairRandomDTO()
        {
            return new FairRandomDTO
            {
                ServerSeed = "test123seed",
                ServerHash = "testhash123",
                UserNonce = 1,
                CombinedHash = "testcombinedhash",
                Fraction1 = 0.5,
                Fraction2 = 0.12345,
                TransactionId = 1
            };
        }

        public static FairRandom GetFairRandom()
        {
            return new FairRandom
            {
                FairRandomId = 1,
                ServerSeed = "test123seed",
                ServerHash = "testhash123",
                UserNonce = 1,
                CombinedHash = "testcombinedhash",
                Fraction1 = 0.5,
                Fraction2 = 0.12345,
            };
        }

        public static List<FairRandom> GetFairRandoms()
        {
            return new List<FairRandom>
            {
                new FairRandom
                {
                    FairRandomId = 1,
                    ServerSeed = "abc123seed456",
                    ServerHash = "hash123abc456def",
                    UserNonce = 1,
                    CombinedHash = "combined123hash456",
                    Fraction1 = 0.12345,
                    Fraction2 = 0.12345,
                },
                new FairRandom
                {
                    FairRandomId = 2,
                    ServerSeed = "xyz789seed012",
                    ServerHash = "hash789xyz012ghi",
                    UserNonce = 2,
                    CombinedHash = "combined789hash012",
                    Fraction1 = 0.67890,
                    Fraction2 = 0.12345,
                },
                new FairRandom
                {
                    FairRandomId = 3,
                    ServerSeed = "def456seed789",
                    ServerHash = "hash456def789jkl",
                    UserNonce = 3,
                    CombinedHash = "combined456hash789",
                    Fraction1 = 0.98765,
                    Fraction2 = 0.12345,
                }
            };
        }
    }
}
