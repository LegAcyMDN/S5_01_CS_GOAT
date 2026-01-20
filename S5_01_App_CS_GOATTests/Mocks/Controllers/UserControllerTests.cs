using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.Controllers;
using Shared.DTO;
using Shared.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;

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
            var adminUser = UserFixture.GetAdminUser();
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
            var normalUser = UserFixture.GetNormalUser();
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
            var users = UserFixture.GetUsers();
            var adminUser = UserFixture.GetAdminUser();

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
            var user = UserFixture.GetNormalUser();
            var userDTO = UserFixture.GetNormalUserDTO();
            var adminUser = UserFixture.GetAdminUser();

            JwtService.AuthentifyController(controller!, adminUser);
            userRepositoryMock!.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<User>>())).ReturnsAsync(user);
            mapperMock!.Setup(m => m.Map<UserDTO>(user)).Returns(userDTO);

            // When
            IActionResult? result = controller!.Get(2).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void Get_ValidIdOwnUser_ReturnsOk()
        {
            // Given
            var user = UserFixture.GetNormalUser();
            var userDTO = UserFixture.GetNormalUserDTO();

            JwtService.AuthentifyController(controller!, user);
            userRepositoryMock!.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<User>>())).ReturnsAsync(user);
            mapperMock!.Setup(m => m.Map<UserDTO>(user)).Returns(userDTO);

            // When
            IActionResult? result = controller!.Get(2).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void Get_DifferentUserIdAsNormalUser_ReturnsForbid()
        {
            // Given
            var normalUser = UserFixture.GetNormalUser();
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
            var adminUser = UserFixture.GetAdminUser();
            JwtService.AuthentifyController(controller!, adminUser);
            userRepositoryMock!.Setup(r => r.GetByIdAsync(999, It.IsAny<QueryOptions<User>>())).ReturnsAsync((User?)null);

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
            var user = UserFixture.GetNormalUser();
            var userDTO = UserFixture.GetNormalUserDTO();

            JwtService.AuthentifyController(controller!, UserFixture.GetAdminUser());

            userRepositoryMock!.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(user);
            mapperMock!.Setup(m => m.Map<UserDTO>(user))
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
            var user = UserFixture.GetNormalUser();
            var userDTO = UserFixture.GetNormalUserDTO();

            JwtService.AuthentifyController(controller!, UserFixture.GetNormalUser());

            userRepositoryMock!.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(user);
            mapperMock!.Setup(m => m.Map<UserDTO>(user))
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

            userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(recentUsers);

            // When
            IActionResult? result = controller!.GetCount().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            Assert.AreEqual(3, okResult.Value);
        }

        [TestMethod]
        public void GetCount_NoRecentlyLoginUsers_ReturnsZero()
        {
            // Given
            var emptyList = new List<User>();
            userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(emptyList);

            // When
            IActionResult? result = controller!.GetCount().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
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

            userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>>()))
                .ReturnsAsync(new List<User>());

            // When
            IActionResult? result = controller!.GetCount().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            Assert.AreEqual(0, okResult.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetCount_RepositoryThrowsException_ThrowsException()
        {
            // Given
            userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>?>()))
                .ThrowsAsync(new Exception("Database error"));

            // When
            controller!.GetCount().GetAwaiter().GetResult();
        }

        [TestMethod]
        public void GetCount_SingleRecentUser_ReturnsOne()
        {
            // Given
            var singleUser = new List<User>
            {
                new User { UserId = 1, Login = "user1", LastLogin = DateTime.UtcNow.AddMinutes(-1) }
            };

            userRepositoryMock!.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<User>?>()))
                .ReturnsAsync(singleUser);

            // When
            IActionResult? result = controller!.GetCount().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            Assert.AreEqual(1, okResult.Value);
        }

        #endregion

        #region Login Tests

        [TestMethod]
        public void Login_ValidCredentials_ReturnsOkWithAuthDTO()
        {
            // Given
            var loginDTO = UserFixture.GetValidLoginDTO();
            var user = UserFixture.GetNormalUser();
            var authDTO = UserFixture.GetAuthDTO();

            userRepositoryMock!.Setup(r => r.Login(loginDTO))
                .ReturnsAsync(user);
            userRepositoryMock.Setup(r => r.Auth(user, configurationMock!.Object, loginDTO.Remember))
                .ReturnsAsync(authDTO);

            // When
            IActionResult? result = controller!.Login(loginDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            Assert.AreEqual(authDTO, okResult.Value);
            userRepositoryMock.Verify(r => r.Login(loginDTO), Times.Once);
        }

        [TestMethod]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Given
            var loginDTO = UserFixture.GetInvalidLoginDTO();

            userRepositoryMock!.Setup(r => r.Login(loginDTO))
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
            userRepositoryMock!.Setup(r => r.Login(It.IsAny<LoginDTO>()))
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
            var tokenDTO = UserFixture.GetRememberToken();
            var user = UserFixture.GetNormalUser();
            var authDTO = UserFixture.GetAuthDTO();

            userRepositoryMock!.Setup(r => r.Recall(tokenDTO))
                .ReturnsAsync(user);
            userRepositoryMock.Setup(r => r.Auth(user, configurationMock!.Object, null))
                .ReturnsAsync(authDTO);

            // When
            IActionResult? result = controller!.Recall(tokenDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            Assert.AreEqual(authDTO, okResult.Value);
            userRepositoryMock.Verify(r => r.Recall(tokenDTO), Times.Once);
        }

        [TestMethod]
        public void Recall_InvalidRememberToken_ReturnsUnauthorized()
        {
            // Given
            var tokenDTO = new TokenDTO { TokenValue = "invalid-token" };

            userRepositoryMock!.Setup(r => r.Recall(tokenDTO))
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

            userRepositoryMock!.Setup(r => r.Recall(expiredToken))
                .ReturnsAsync((User?)null);

            // When
            IActionResult? result = controller!.Recall(expiredToken).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        #endregion
    }
}