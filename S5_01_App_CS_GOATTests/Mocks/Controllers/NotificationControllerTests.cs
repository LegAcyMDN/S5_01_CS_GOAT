using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class NotificationControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<Notification, int>>? notificationRepositoryMock;
        private Mock<IDataRepository<GlobalNotification, int>>? globalNotificationRepositoryMock;
        private Mock<IDataRepository<UserNotification, int>>? userNotificationRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private NotificationController? controller;

        private User? admin;
        private User? normalUser;
        private GlobalNotification? globalNotification;
        private UserNotification? userNotification;
        private List<Notification>? allNotifications;
        private List<GlobalNotification>? globalNotifications;
        private List<UserNotification>? userNotifications;
        private NotificationDTO? notificationDTO;
        private List<NotificationDTO>? notificationDTOs;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            notificationRepositoryMock = new Mock<IDataRepository<Notification, int>>();
            globalNotificationRepositoryMock = new Mock<IDataRepository<GlobalNotification, int>>();
            userNotificationRepositoryMock = new Mock<IDataRepository<UserNotification, int>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            globalNotification = NotificationFixture.GetGlobalNotification();
            userNotification = NotificationFixture.GetUserNotification();
            allNotifications = NotificationFixture.GetAllNotifications();
            globalNotifications = NotificationFixture.GetGlobalNotifications();
            userNotifications = NotificationFixture.GetUserNotifications();
            notificationDTO = NotificationFixture.GetNotificationDTO();
            notificationDTOs = NotificationFixture.GetNotificationDTOs();

            controller = new NotificationController(
                mapperMock.Object,
                notificationRepositoryMock.Object,
                globalNotificationRepositoryMock.Object,
                userNotificationRepositoryMock.Object,
                configurationMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region GetAll Tests

        [TestMethod]
        public void GetAll_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            notificationRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetAll_AsNonAdmin_ReturnsForbidden()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            notificationRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetAll_AsAdmin_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            notificationRepositoryMock.Setup(r => r.GetAllAsync(null))
                                      .ReturnsAsync(allNotifications);
            mapperMock.Setup(m => m.Map<IEnumerable<NotificationDTO>>(allNotifications))
                      .Returns(notificationDTOs);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            notificationRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        [TestMethod]
        public void GetAll_AsAdmin_NoNotifications_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            notificationRepositoryMock.Setup(r => r.GetAllAsync(null))
                                      .ReturnsAsync(new List<Notification>());

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            notificationRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion

        #region GetRelevant Tests

        [TestMethod]
        public void GetRelevant_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetRelevant().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            globalNotificationRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
            userNotificationRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetRelevant_Authenticated_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            globalNotificationRepositoryMock.Setup(r => r.GetAllAsync(null))
                                            .ReturnsAsync(globalNotifications);
            userNotificationRepositoryMock.Setup(r => r.GetAllAsync(null))
                                          .ReturnsAsync(userNotifications);
            mapperMock.Setup(m => m.Map<IEnumerable<NotificationDTO>>(It.IsAny<List<Notification>>()))
                      .Returns(notificationDTOs);

            // When
            IActionResult? result = controller.GetRelevant().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            globalNotificationRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
            userNotificationRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion

        #region GetDetails Tests

        [TestMethod]
        public void GetDetails_ExistingNotification_ReturnsOk()
        {
            // Given
            int notificationId = 1;
            notificationRepositoryMock.Setup(r => r.GetByIdAsync(notificationId))
                                      .ReturnsAsync(globalNotification);
            mapperMock.Setup(m => m.Map<NotificationDTO>(globalNotification))
                      .Returns(notificationDTO);

            // When
            IActionResult? result = controller.GetDetails(notificationId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            notificationRepositoryMock.Verify(r => r.GetByIdAsync(notificationId), Times.Once);
        }

        [TestMethod]
        public void GetDetails_NonExistingNotification_ReturnsNotFound()
        {
            // Given
            int notificationId = 999;
            notificationRepositoryMock.Setup(r => r.GetByIdAsync(notificationId))
                                      .ReturnsAsync((Notification?)null);

            // When
            IActionResult? result = controller.GetDetails(notificationId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            notificationRepositoryMock.Verify(r => r.GetByIdAsync(notificationId), Times.Once);
        }

        #endregion
    }
}
