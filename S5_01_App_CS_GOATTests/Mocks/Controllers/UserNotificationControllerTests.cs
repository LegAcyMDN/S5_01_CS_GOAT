using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class UserNotificationControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<UserNotification, int>>? notificationRepositoryMock;
        private Mock<ITypeRepository<NotificationType>>? notificationTypeRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private UserNotificationController? controller;

        private User? admin;
        private User? normalUser;
        private UserNotification? userNotification;
        private NotificationDTO? notificationDTO;
        private NotificationType? notificationType;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            notificationRepositoryMock = new Mock<IDataRepository<UserNotification, int>>();
            notificationTypeRepositoryMock = new Mock<ITypeRepository<NotificationType>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            userNotification = NotificationFixture.GetUserNotification();
            notificationDTO = NotificationFixture.GetSingleNotificationDTO();
            notificationType = NotificationFixture.GetNotificationType();

            controller = new UserNotificationController(
                mapperMock.Object,
                notificationRepositoryMock.Object,
                notificationTypeRepositoryMock.Object,
                configurationMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region POST Tests

        [TestMethod]
        public void Create_AsAdmin_ValidNotification_ReturnsCreatedAtRoute()
        {
            JwtService.AuthentifyController(controller, admin);
            notificationTypeRepositoryMock.Setup(r => r.GetTypeByNameAsync(notificationDTO.NotificationTypeName))
                                           .ReturnsAsync(notificationType);
            mapperMock.Setup(m => m.Map<UserNotification>(notificationDTO))
                       .Returns(userNotification);
            notificationRepositoryMock.Setup(r => r.AddAsync(userNotification))
                                       .ReturnsAsync(userNotification);
            mapperMock.Setup(m => m.Map<NotificationDTO>(userNotification))
                       .Returns(notificationDTO);

            // When
            IActionResult? result = controller.Create(notificationDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
            notificationRepositoryMock.Verify(r => r.AddAsync(userNotification), Times.Once);
        }

        [TestMethod]
        public void Create_AsNonAdmin_ReturnsForbidden()
        {
            JwtService.AuthentifyController(controller, normalUser);

            // When
            IActionResult? result = controller.Create(notificationDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            notificationRepositoryMock.Verify(r => r.AddAsync(userNotification), Times.Never);
        }

        [TestMethod]
        public void Create_InvalidModelState_ReturnsBadRequest()
        {
            JwtService.AuthentifyController(controller, admin);
            controller.ModelState.AddModelError("NotificationSummary", "Required");
            NotificationDTO emptyNotificationDTO = NotificationFixture.GetEmptyNotificationDTO();

            // When
            IActionResult? result = controller.Create(emptyNotificationDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            notificationRepositoryMock.Verify(r => r.AddAsync(userNotification), Times.Never);
        }

        [TestMethod]
        public void Create_InvalidNotificationType_ReturnsBadRequest()
        {
            JwtService.AuthentifyController(controller, admin);
            notificationTypeRepositoryMock.Setup(r => r.GetTypeByNameAsync(notificationDTO.NotificationTypeName))
                                           .ReturnsAsync((NotificationType?)null);

            // When
            IActionResult? result = controller.Create(notificationDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            notificationTypeRepositoryMock.Verify(r => r.GetTypeByNameAsync(notificationDTO.NotificationTypeName), Times.Once);
            notificationRepositoryMock.Verify(r => r.AddAsync(userNotification), Times.Never);
        }

        #endregion
    }
}
