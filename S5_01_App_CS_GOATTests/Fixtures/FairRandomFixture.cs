using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

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
                    Fraction = 0.12345,
                    TransactionId = 1,
                    ItemWearId = null,
                    ItemUserId = null
                },
                new FairRandomDTO
                {
                    ServerSeed = "xyz789seed012",
                    ServerHash = "hash789xyz012ghi",
                    UserNonce = 2,
                    CombinedHash = "combined789hash012",
                    Fraction = 0.67890,
                    TransactionId = 2,
                    ItemWearId = null,
                    ItemUserId = null
                },
                new FairRandomDTO
                {
                    ServerSeed = "def456seed789",
                    ServerHash = "hash456def789jkl",
                    UserNonce = 3,
                    CombinedHash = "combined456hash789",
                    Fraction = 0.98765,
                    TransactionId = null,
                    ItemWearId = 1,
                    ItemUserId = 2
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
                Fraction = 0.5,
                TransactionId = 1,
                ItemWearId = null,
                ItemUserId = null
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
                Fraction = 0.5
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
                    Fraction = 0.12345
                },
                new FairRandom
                {
                    FairRandomId = 2,
                    ServerSeed = "xyz789seed012",
                    ServerHash = "hash789xyz012ghi",
                    UserNonce = 2,
                    CombinedHash = "combined789hash012",
                    Fraction = 0.67890
                },
                new FairRandom
                {
                    FairRandomId = 3,
                    ServerSeed = "def456seed789",
                    ServerHash = "hash456def789jkl",
                    UserNonce = 3,
                    CombinedHash = "combined456hash789",
                    Fraction = 0.98765
                }
            };
        }
    }
}
