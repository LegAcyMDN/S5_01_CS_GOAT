using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;
using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class UserControllerTests
    {
        private Mock<IUserRepository>? userRepositoryMock;
        private Mock<ISendingRepository>? sendingRepositoryMock;
        private Mock<IMapper>? mapperMock;
        private Mock<IConfiguration>? configurationMock;
        private UserController? controller;

        [TestInitialize]
        public void Initialize()
        {
            userRepositoryMock = new Mock<IUserRepository>();
            sendingRepositoryMock = new Mock<ISendingRepository>();
            mapperMock = new Mock<IMapper>();
            configurationMock = new Mock<IConfiguration>();

            controller = new UserController(
                userRepositoryMock.Object,
                sendingRepositoryMock.Object,
                mapperMock.Object,
                configurationMock.Object
            );
        }

        #region GetAll Tests

        [TestMethod]
        public void GetAll_AdminUserAuthenticated_ReturnsOk()
        {
            // Given
            User adminUser = UserFixture.GetAdminUser();
            JwtService.AuthentifyController(controller!, adminUser);

            // When
            IActionResult? result = controller!.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetAll_NonAdminUserAuthenticated_ReturnsForbid()
        {
            // Given
            User normalUser = UserFixture.GetNormalUser();
            JwtService.AuthentifyController(controller!, normalUser);

            // When
            IActionResult? result = controller!.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public void GetAll_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Given
            Thread.CurrentPrincipal = null;

            // When
            IActionResult? result = controller!.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GetAll_AdminUserAuthenticated_ReturnsOkWithUsers()
        {
            // Given
            _ = UserFixture.GetUsers();
            User adminUser = UserFixture.GetAdminUser();

            JwtService.AuthentifyController(controller!, adminUser);

            // When
            IActionResult? result = controller!.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        #endregion

        #region Get (Details) Tests

        [TestMethod]
        public void Get_ValidIdAsAdmin_ReturnsOk()
        {
            // Given
            User user = UserFixture.GetNormalUser();
            UserDTO userDTO = UserFixture.GetNormalUserDTO();
            User adminUser = UserFixture.GetAdminUser();

            JwtService.AuthentifyController(controller!, adminUser);
            _ = userRepositoryMock!.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<User>>())).ReturnsAsync(user);
            _ = mapperMock!.Setup(m => m.Map<UserDTO>(user)).Returns(userDTO);

            // When
            IActionResult? result = controller!.Get(2).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void Get_ValidIdOwnUser_ReturnsOk()
        {
            // Given
            User user = UserFixture.GetNormalUser();
            UserDTO userDTO = UserFixture.GetNormalUserDTO();

            JwtService.AuthentifyController(controller!, user);
            _ = userRepositoryMock!.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<User>>())).ReturnsAsync(user);
            _ = mapperMock!.Setup(m => m.Map<UserDTO>(user)).Returns(userDTO);

            // When
            IActionResult? result = controller!.Get(2).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void Get_DifferentUserIdAsNormalUser_ReturnsForbid()
        {
            // Given
            User normalUser = UserFixture.GetNormalUser();
            JwtService.AuthentifyController(controller!, normalUser);

            // When
            IActionResult? result = controller!.Get(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public void Get_UserNotFound_ReturnsNotFound()
        {
            // Given
            User adminUser = UserFixture.GetAdminUser();
            JwtService.AuthentifyController(controller!, adminUser);
            _ = userRepositoryMock!.Setup(r => r.GetByIdAsync(999, It.IsAny<QueryOptions<User>>())).ReturnsAsync((User?)null);

            // When
            IActionResult? result = controller!.Get(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Get_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Given
            Thread.CurrentPrincipal = null;

            // When
            IActionResult? result = controller!.Get(2).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void Get_ValidIdAsAdmin_ReturnsOkWithUser()
        {
            // Given
            User user = UserFixture.GetNormalUser();
            UserDTO userDTO = UserFixture.GetNormalUserDTO();

            JwtService.AuthentifyController(controller!, UserFixture.GetAdminUser());

            _ = userRepositoryMock!.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(user);
            _ = mapperMock!.Setup(m => m.Map<UserDTO>(user))
                .Returns(userDTO);

            // When
            IActionResult? result = controller!.Get(2).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void Get_ValidIdOwnUser_ReturnsOkWithUser()
        {
            // Given
            User user = UserFixture.GetNormalUser();
            UserDTO userDTO = UserFixture.GetNormalUserDTO();

            JwtService.AuthentifyController(controller!, UserFixture.GetNormalUser());

            _ = userRepositoryMock!.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(user);
            _ = mapperMock!.Setup(m => m.Map<UserDTO>(user))
                .Returns(userDTO);

            // When
            IActionResult? result = controller!.Get(2).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        #endregion

        #region GetCount Tests

        public void GetCount_WithRecentlyLoginUsers_ReturnsOkWithCount()
        {
            // Given
            var recentUsers = new List<User>
            {
                new User { UserId = 1, Login = "user1", LastLogin = DateTime.UtcNow.AddMinutes(-5) },
                new User { UserId = 2, Login = "user2", LastLogin = DateTime.UtcNow.AddMinutes(-10) },
                new User { UserId = 3, Login = "user3", LastLogin = DateTime.UtcNow.AddMinutes(-14) }
            };

            _ = userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(recentUsers);

            // When
            IActionResult? result = controller!.GetCount().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(3, okResult.Value);
        }

        [TestMethod]
        public void GetCount_NoRecentlyLoginUsers_ReturnsZero()
        {
            // Given
            var emptyList = new List<User>();
            _ = userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(emptyList);

            // When
            IActionResult? result = controller!.GetCount().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(0, okResult.Value);
        }

        [TestMethod]
        public void GetCount_UsersNotWithin15Minutes_ReturnsEmptyCount()
        {
            // Given
            var oldUsers = new List<User>
            {
                new User { UserId = 1, Login = "user1", LastLogin = DateTime.UtcNow.AddMinutes(-20) },
                new User { UserId = 2, Login = "user2", LastLogin = DateTime.UtcNow.AddMinutes(-30) }
            };

            _ = userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(new List<User>());

            // When
            IActionResult? result = controller!.GetCount().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(0, okResult.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetCount_RepositoryThrowsException_ThrowsException()
        {
            // Given
            _ = userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>?>()))
                .ThrowsAsync(new Exception("Database error"));

            // When
            _ = controller!.GetCount().GetAwaiter().GetResult();
        }

        [TestMethod]
        public void GetCount_SingleRecentUser_ReturnsOne()
        {
            // Given
            var singleUser = new List<User>
            {
                new User { UserId = 1, Login = "user1", LastLogin = DateTime.UtcNow.AddMinutes(-1) }
            };

            _ = userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>?>()))
                .ReturnsAsync(singleUser);

            // When
            IActionResult? result = controller!.GetCount().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(1, okResult.Value);
        }

        #endregion

        #region Login Tests

        [TestMethod]
        public void Login_ValidCredentials_ReturnsOkWithAuthDTO()
        {
            // Given
            LoginDTO loginDTO = UserFixture.GetValidLoginDTO();
            User user = UserFixture.GetNormalUser();
            AuthDTO authDTO = UserFixture.GetAuthDTO();

            _ = userRepositoryMock!.Setup(r => r.Login(loginDTO))
                .ReturnsAsync(user);
            _ = userRepositoryMock.Setup(r => r.Auth(user, configurationMock!.Object, loginDTO.Remember))
                .ReturnsAsync(authDTO);

            // When
            IActionResult? result = controller!.Login(loginDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(authDTO, okResult.Value);
            userRepositoryMock.Verify(r => r.Login(loginDTO), Times.Once);
        }

        [TestMethod]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Given
            LoginDTO loginDTO = UserFixture.GetInvalidLoginDTO();

            _ = userRepositoryMock!.Setup(r => r.Login(loginDTO))
                .ReturnsAsync((User?)null);

            // When
            IActionResult? result = controller!.Login(loginDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            userRepositoryMock.Verify(r => r.Auth(It.IsAny<User>(), It.IsAny<IConfiguration>(), It.IsAny<int?>()), Times.Never);
        }

        [TestMethod]
        public void Login_NullLoginDTO_ReturnsUnauthorized()
        {
            // Given
            _ = userRepositoryMock!.Setup(r => r.Login(It.IsAny<LoginDTO>()))
                .ReturnsAsync((User?)null);

            // When
            IActionResult? result = controller!.Login(new LoginDTO()).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        #endregion

        #region Recall Tests

        [TestMethod]
        public void Recall_ValidRememberToken_ReturnsOkWithAuthDTO()
        {
            // Given
            TokenDTO tokenDTO = UserFixture.GetRememberToken();
            User user = UserFixture.GetNormalUser();
            AuthDTO authDTO = UserFixture.GetAuthDTO();

            _ = userRepositoryMock!.Setup(r => r.Recall(tokenDTO))
                .ReturnsAsync(user);
            _ = userRepositoryMock.Setup(r => r.Auth(user, configurationMock!.Object, null))
                .ReturnsAsync(authDTO);

            // When
            IActionResult? result = controller!.Recall(tokenDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(authDTO, okResult.Value);
            userRepositoryMock.Verify(r => r.Recall(tokenDTO), Times.Once);
        }

        [TestMethod]
        public void Recall_InvalidRememberToken_ReturnsUnauthorized()
        {
            // Given
            var tokenDTO = new TokenDTO { TokenValue = "invalid-token" };

            _ = userRepositoryMock!.Setup(r => r.Recall(tokenDTO))
                .ReturnsAsync((User?)null);

            // When
            IActionResult? result = controller!.Recall(tokenDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            userRepositoryMock.Verify(r => r.Auth(It.IsAny<User>(), It.IsAny<IConfiguration>(), It.IsAny<int?>()), Times.Never);
        }

        [TestMethod]
        public void Recall_ExpiredRememberToken_ReturnsUnauthorized()
        {
            // Given
            var expiredToken = new TokenDTO
            {
                TokenValue = "expired-token",
                TokenExpiry = DateTime.UtcNow.AddDays(-1)
            };

            _ = userRepositoryMock!.Setup(r => r.Recall(expiredToken))
                .ReturnsAsync((User?)null);

            // When
            IActionResult? result = controller!.Recall(expiredToken).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        #endregion
    }
}