using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOATTests.Mocks.Services
{
    [TestClass()]
    public class SecurityServiceTests
    {
        #region GenerateSeed Tests

        [TestMethod]
        public void GenerateSeed_DefaultLength_Returns16CharacterString()
        {
            // When
            string seed = SecurityService.GenerateSeed();

            // Then
            Assert.IsNotNull(seed);
            Assert.AreEqual(16, seed.Length);
        }

        [TestMethod]
        public void GenerateSeed_CustomLength_ReturnsCorrectLength()
        {
            // When
            string seed = SecurityService.GenerateSeed(32);

            // Then
            Assert.AreEqual(32, seed.Length);
        }

        [TestMethod]
        public void GenerateSeed_ZeroLength_ReturnsEmptyString()
        {
            // When
            string seed = SecurityService.GenerateSeed(0);

            // Then
            Assert.AreEqual(0, seed.Length);
        }

        [TestMethod]
        public void GenerateSeed_MultipleGenerations_ProducesDifferentSeeds()
        {
            // When
            string seed1 = SecurityService.GenerateSeed();
            string seed2 = SecurityService.GenerateSeed();
            string seed3 = SecurityService.GenerateSeed();

            // Then
            Assert.AreNotEqual(seed1, seed2);
            Assert.AreNotEqual(seed2, seed3);
            Assert.AreNotEqual(seed1, seed3);
        }

        [TestMethod]
        public void GenerateSeed_ContainsOnlyValidCharacters()
        {
            // When
            string seed = SecurityService.GenerateSeed(100);

            // Then
            Assert.IsTrue(seed.All(c => char.IsLetterOrDigit(c)));
        }

        #endregion

        #region GenerateToken Tests

        [TestMethod]
        public void GenerateToken_DefaultLength_ReturnsBase64String()
        {
            // When
            string token = SecurityService.GenerateToken();

            // Then
            Assert.IsNotNull(token);
            Assert.IsTrue(token.Length > 0);
            // Verify it's valid Base64
            byte[] decoded = Convert.FromBase64String(token);
            Assert.AreEqual(64, decoded.Length);
        }

        [TestMethod]
        public void GenerateToken_CustomLength_ReturnsCorrectByteLength()
        {
            // When
            string token = SecurityService.GenerateToken(128);

            // Then
            byte[] decoded = Convert.FromBase64String(token);
            Assert.AreEqual(128, decoded.Length);
        }

        [TestMethod]
        public void GenerateToken_MultipleGenerations_ProducesDifferentTokens()
        {
            // When
            string token1 = SecurityService.GenerateToken();
            string token2 = SecurityService.GenerateToken();
            string token3 = SecurityService.GenerateToken();

            // Then
            Assert.AreNotEqual(token1, token2);
            Assert.AreNotEqual(token2, token3);
            Assert.AreNotEqual(token1, token3);
        }

        [TestMethod]
        public void GenerateToken_SmallLength_Works()
        {
            // When
            string token = SecurityService.GenerateToken(8);

            // Then
            byte[] decoded = Convert.FromBase64String(token);
            Assert.AreEqual(8, decoded.Length);
        }

        #endregion

        #region HashString Tests

        [TestMethod]
        public void HashString_ValidInput_ReturnsBase64Hash()
        {
            // Given
            string input = "test string";

            // When
            string hash = SecurityService.HashString(input);

            // Then
            Assert.IsNotNull(hash);
            Assert.IsTrue(hash.Length > 0);
            byte[] decoded = Convert.FromBase64String(hash);
            Assert.AreEqual(32, decoded.Length); // SHA256 produces 32 bytes
        }

        [TestMethod]
        public void HashString_SameInput_ProducesSameHash()
        {
            // Given
            string input = "consistent input";

            // When
            string hash1 = SecurityService.HashString(input);
            string hash2 = SecurityService.HashString(input);

            // Then
            Assert.AreEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashString_DifferentInput_ProducesDifferentHash()
        {
            // Given
            string input1 = "input one";
            string input2 = "input two";

            // When
            string hash1 = SecurityService.HashString(input1);
            string hash2 = SecurityService.HashString(input2);

            // Then
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashString_NullInput_ThrowsArgumentException()
        {
            // When/Then
            _ = Assert.ThrowsException<ArgumentException>(() =>
            {
                _ = SecurityService.HashString(null);
            });
        }

        [TestMethod]
        public void HashString_EmptyInput_ThrowsArgumentException()
        {
            // When/Then
            _ = Assert.ThrowsException<ArgumentException>(() =>
            {
                _ = SecurityService.HashString(string.Empty);
            });
        }

        [TestMethod]
        public void HashString_LongInput_Works()
        {
            // Given
            string input = new string('a', 10000);

            // When
            string hash = SecurityService.HashString(input);

            // Then
            Assert.IsNotNull(hash);
            byte[] decoded = Convert.FromBase64String(hash);
            Assert.AreEqual(32, decoded.Length);
        }

        #endregion

        #region HashAndSalt Tests

        [TestMethod]
        public void HashAndSalt_ValidInputs_ReturnsBase64Hash()
        {
            // Given
            string password = "MySecurePassword123!";
            string salt = SecurityService.GenerateToken(16);

            // When
            string hash = SecurityService.HashAndSalt(password, salt);

            // Then
            Assert.IsNotNull(hash);
            Assert.IsTrue(hash.Length > 0);
            byte[] decoded = Convert.FromBase64String(hash);
            Assert.AreEqual(32, decoded.Length);
        }

        [TestMethod]
        public void HashAndSalt_SamePasswordAndSalt_ProducesSameHash()
        {
            // Given
            string password = "MyPassword123";
            string salt = SecurityService.GenerateToken(16);

            // When
            string hash1 = SecurityService.HashAndSalt(password, salt);
            string hash2 = SecurityService.HashAndSalt(password, salt);

            // Then
            Assert.AreEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashAndSalt_DifferentPasswords_ProducesDifferentHash()
        {
            // Given
            string password1 = "Password1";
            string password2 = "Password2";
            string salt = SecurityService.GenerateToken(16);

            // When
            string hash1 = SecurityService.HashAndSalt(password1, salt);
            string hash2 = SecurityService.HashAndSalt(password2, salt);

            // Then
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashAndSalt_DifferentSalts_ProducesDifferentHash()
        {
            // Given
            string password = "SamePassword";
            string salt1 = SecurityService.GenerateToken(16);
            string salt2 = SecurityService.GenerateToken(16);

            // When
            string hash1 = SecurityService.HashAndSalt(password, salt1);
            string hash2 = SecurityService.HashAndSalt(password, salt2);

            // Then
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashAndSalt_NullPassword_ThrowsArgumentException()
        {
            // Given
            string salt = SecurityService.GenerateToken(16);

            // When/Then
            _ = Assert.ThrowsException<ArgumentException>(() =>
            {
                _ = SecurityService.HashAndSalt(null, salt);
            });
        }

        [TestMethod]
        public void HashAndSalt_EmptyPassword_ThrowsArgumentException()
        {
            // Given
            string salt = SecurityService.GenerateToken(16);

            // When/Then
            _ = Assert.ThrowsException<ArgumentException>(() =>
            {
                _ = SecurityService.HashAndSalt(string.Empty, salt);
            });
        }

        [TestMethod]
        public void HashAndSalt_NullSalt_ThrowsArgumentException()
        {
            // Given
            string password = "Password123";

            // When/Then
            _ = Assert.ThrowsException<ArgumentException>(() =>
            {
                _ = SecurityService.HashAndSalt(password, null);
            });
        }

        [TestMethod]
        public void HashAndSalt_EmptySalt_ThrowsArgumentException()
        {
            // Given
            string password = "Password123";

            // When/Then
            _ = Assert.ThrowsException<ArgumentException>(() =>
            {
                _ = SecurityService.HashAndSalt(password, string.Empty);
            });
        }

        [TestMethod]
        public void HashAndSalt_ComplexPassword_Works()
        {
            // Given
            string password = "P@ssw0rd!#$%^&*()_+-=[]{}|;':\",./<>?";
            string salt = SecurityService.GenerateToken(16);

            // When
            string hash = SecurityService.HashAndSalt(password, salt);

            // Then
            Assert.IsNotNull(hash);
            byte[] decoded = Convert.FromBase64String(hash);
            Assert.AreEqual(32, decoded.Length);
        }

        [TestMethod]
        public void HashAndSalt_LongPassword_Works()
        {
            // Given
            string password = new string('x', 1000);
            string salt = SecurityService.GenerateToken(16);

            // When
            string hash = SecurityService.HashAndSalt(password, salt);

            // Then
            Assert.IsNotNull(hash);
            byte[] decoded = Convert.FromBase64String(hash);
            Assert.AreEqual(32, decoded.Length);
        }

        #endregion

        #region VerifyPassword Tests

        [TestMethod]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            // Given
            string password = "MySecurePassword123!";
            string salt = SecurityService.GenerateToken(16);
            string hash = SecurityService.HashAndSalt(password, salt);

            // When
            bool? result = SecurityService.VerifyPassword(password, hash, salt);

            // Then
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public void VerifyPassword_IncorrectPassword_ReturnsFalse()
        {
            // Given
            string correctPassword = "CorrectPassword123";
            string incorrectPassword = "WrongPassword123";
            string salt = SecurityService.GenerateToken(16);
            string hash = SecurityService.HashAndSalt(correctPassword, salt);

            // When
            bool? result = SecurityService.VerifyPassword(incorrectPassword, hash, salt);

            // Then
            Assert.IsTrue(result == false);
        }

        [TestMethod]
        public void VerifyPassword_CaseSensitive_ReturnsFalse()
        {
            // Given
            string password = "Password123";
            string differentCase = "PASSWORD123";
            string salt = SecurityService.GenerateToken(16);
            string hash = SecurityService.HashAndSalt(password, salt);

            // When
            bool? result = SecurityService.VerifyPassword(differentCase, hash, salt);

            // Then
            Assert.IsTrue(result == false);
        }

        [TestMethod]
        public void VerifyPassword_WrongSalt_ReturnsFalse()
        {
            // Given
            string password = "Password123";
            string salt = SecurityService.GenerateToken(16);
            string wrongSalt = SecurityService.GenerateToken(16);
            string hash = SecurityService.HashAndSalt(password, salt);

            // When
            bool? result = SecurityService.VerifyPassword(password, hash, wrongSalt);

            // Then
            Assert.IsTrue(result == false);
        }

        [TestMethod]
        public void VerifyPassword_EmptyPassword_ReturnsNull()
        {
            // Given
            string salt = SecurityService.GenerateToken(16);
            string hash = SecurityService.HashAndSalt("password", salt);

            // When
            bool? result = SecurityService.VerifyPassword(string.Empty, hash, salt);

            // Then
            Assert.IsNull(result);
        }

        [TestMethod]
        public void VerifyPassword_NullPassword_ReturnsNull()
        {
            // Given
            string salt = SecurityService.GenerateToken(16);
            string hash = SecurityService.HashAndSalt("password", salt);

            // When
            bool? result = SecurityService.VerifyPassword(null, hash, salt);

            // Then
            Assert.IsNull(result);
        }

        [TestMethod]
        public void VerifyPassword_NullHash_ReturnsNull()
        {
            // Given
            string salt = SecurityService.GenerateToken(16);

            // When
            bool? result = SecurityService.VerifyPassword("password", null, salt);

            // Then
            Assert.IsNull(result);
        }

        [TestMethod]
        public void VerifyPassword_NullSalt_ReturnsNull()
        {
            // Given
            string hash = "somehash";

            // When
            bool? result = SecurityService.VerifyPassword("password", hash, null);

            // Then
            Assert.IsNull(result);
        }

        #endregion
    }
}
