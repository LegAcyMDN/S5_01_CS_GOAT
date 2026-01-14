using Microsoft.VisualStudio.TestTools.UnitTesting;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOATTests.Fixtures;

namespace S5_01_App_CS_GOATTests.Mocks.Services
{
    [TestClass()]
    public class AuthResultTests
    {
        private User? normalUser;
        private User? adminUser;
        private UserDependantTestClass? userResource;
        private UserDependantTestClass? otherUserResource;
        private UserDependantTestClass? noUserResource;

        [TestInitialize]
        public void Initialize()
        {
            normalUser = UserFixture.GetNormalUser();
            adminUser = UserFixture.GetAdminUser();

            userResource = new UserDependantTestClass { DependantUserId = normalUser.UserId };
            otherUserResource = new UserDependantTestClass { DependantUserId = 999 };
            noUserResource = new UserDependantTestClass { DependantUserId = null };
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_WithoutParameters_CreatesUnauthenticatedResult()
        {
            // When
            var authResult = new AuthResult();

            // Then
            Assert.IsNull(authResult.AuthUserId);
            Assert.IsFalse(authResult.IsAdmin);
            Assert.IsFalse(authResult.IsAuthenticated);
        }

        [TestMethod]
        public void Constructor_WithUserId_CreatesAuthenticatedResult()
        {
            // When
            var authResult = new AuthResult(normalUser.UserId);

            // Then
            Assert.AreEqual(normalUser.UserId, authResult.AuthUserId);
            Assert.IsFalse(authResult.IsAdmin);
            Assert.IsTrue(authResult.IsAuthenticated);
        }

        [TestMethod]
        public void Constructor_WithUserIdAndAdmin_CreatesAdminResult()
        {
            // When
            var authResult = new AuthResult(adminUser.UserId, true);

            // Then
            Assert.AreEqual(adminUser.UserId, authResult.AuthUserId);
            Assert.IsTrue(authResult.IsAdmin);
            Assert.IsTrue(authResult.IsAuthenticated);
        }

        [TestMethod]
        public void Constructor_WithNullUserId_CreatesUnauthenticatedResult()
        {
            // When
            var authResult = new AuthResult(null);

            // Then
            Assert.IsNull(authResult.AuthUserId);
            Assert.IsFalse(authResult.IsAuthenticated);
        }

        #endregion

        #region IsAuthenticated Tests

        [TestMethod]
        public void IsAuthenticated_WithUserId_ReturnsTrue()
        {
            // When
            var authResult = new AuthResult(123);

            // Then
            Assert.IsTrue(authResult.IsAuthenticated);
        }

        [TestMethod]
        public void IsAuthenticated_WithoutUserId_ReturnsFalse()
        {
            // When
            var authResult = new AuthResult();

            // Then
            Assert.IsFalse(authResult.IsAuthenticated);
        }

        #endregion

        #region IsAllowed Tests

        [TestMethod]
        public void IsAllowed_ResourceWithNoDependantUser_ReturnsTrue()
        {
            // Given
            var authResult = new AuthResult();

            // When
            bool allowed = authResult.IsAllowed(noUserResource, false);

            // Then
            Assert.IsTrue(allowed);
        }

        [TestMethod]
        public void IsAllowed_UnauthenticatedUser_ReturnsFalse()
        {
            // Given
            var authResult = new AuthResult();

            // When
            bool allowed = authResult.IsAllowed(userResource, false);

            // Then
            Assert.IsFalse(allowed);
        }

        [TestMethod]
        public void IsAllowed_MatchingUserId_ReturnsTrue()
        {
            // Given
            var authResult = new AuthResult(normalUser.UserId);

            // When
            bool allowed = authResult.IsAllowed(userResource, false);

            // Then
            Assert.IsTrue(allowed);
        }

        [TestMethod]
        public void IsAllowed_DifferentUserId_ReturnsFalse()
        {
            // Given
            var authResult = new AuthResult(normalUser.UserId);

            // When
            bool allowed = authResult.IsAllowed(otherUserResource, false);

            // Then
            Assert.IsFalse(allowed);
        }

        [TestMethod]
        public void IsAllowed_AdminWithoutOverride_ReturnsFalse()
        {
            // Given
            var authResult = new AuthResult(adminUser.UserId, true);

            // When
            bool allowed = authResult.IsAllowed(otherUserResource, false);

            // Then
            Assert.IsFalse(allowed);
        }

        [TestMethod]
        public void IsAllowed_AdminWithOverride_ReturnsTrue()
        {
            // Given
            var authResult = new AuthResult(adminUser.UserId, true);

            // When
            bool allowed = authResult.IsAllowed(otherUserResource, true);

            // Then
            Assert.IsTrue(allowed);
        }

        [TestMethod]
        public void IsAllowed_NonAdminWithOverride_ReturnsFalseForOtherUser()
        {
            // Given
            var authResult = new AuthResult(normalUser.UserId, false);

            // When
            bool allowed = authResult.IsAllowed(otherUserResource, true);

            // Then
            Assert.IsFalse(allowed);
        }

        [TestMethod]
        public void IsAllowed_AdminWithOwnResource_ReturnsTrue()
        {
            // Given
            var ownResource = new UserDependantTestClass { DependantUserId = adminUser.UserId };
            var authResult = new AuthResult(adminUser.UserId, true);

            // When
            bool allowed = authResult.IsAllowed(ownResource, false);

            // Then
            Assert.IsTrue(allowed);
        }

        #endregion

        // Test helper class implementing IUserDependant
        private class UserDependantTestClass : IUserDependant
        {
            public int? DependantUserId { get; set; }
        }
    }
}
