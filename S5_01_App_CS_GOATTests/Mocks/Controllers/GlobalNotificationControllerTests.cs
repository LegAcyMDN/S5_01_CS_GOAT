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
using System.Threading;
using System.Threading.Tasks;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class GlobalNotificationControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<GlobalNotification, int>>? globalNotificationRepositoryMock;
        private Mock<ITypeRepository<NotificationType>>? notificationTypeRepositoryMock;
        private GlobalNotificationController? controller;
        private Mock<IConfiguration>? configurationMock;

        private User? admin;
        private User? normalUser;
        private NotificationType? notificationType;
        private NotificationDTO? notificationDTO;
        private GlobalNotification? globalNotification;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            globalNotificationRepositoryMock = new Mock<IDataRepository<GlobalNotification, int>>();
            notificationTypeRepositoryMock = new Mock<ITypeRepository<NotificationType>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            notificationType = NotificationFixture.GetNotificationType();
            notificationDTO = NotificationFixture.GetSingleNotificationDTO();
            globalNotification = NotificationFixture.GetGlobalNotification();

            controller = new GlobalNotificationController(
                mapperMock.Object,
                globalNotificationRepositoryMock.Object,
                notificationTypeRepositoryMock.Object,
                configurationMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region Create Tests

        [TestMethod]
        public void Create_AsAdmin_ValidNotification_ReturnsCreatedAtAction()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            notificationTypeRepositoryMock.Setup(r => r.GetTypeByNameAsync(notificationDTO.NotificationTypeName))
                                          .ReturnsAsync(notificationType);
            mapperMock.Setup(m => m.Map<GlobalNotification>(notificationDTO))
                      .Returns(globalNotification);
            globalNotificationRepositoryMock.Setup(r => r.AddAsync(globalNotification))
                                            .ReturnsAsync(globalNotification);
            mapperMock.Setup(m => m.Map<NotificationDTO>(globalNotification))
                      .Returns(notificationDTO);

            // When
            IActionResult? result = controller.Create(notificationDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            globalNotificationRepositoryMock.Verify(r => r.AddAsync(globalNotification), Times.Once);
        }

        [TestMethod]
        public void Create_AsNonAdmin_ReturnsForbidden()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);

            // When
            IActionResult? result = controller.Create(notificationDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public void Create_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.Create(notificationDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void Create_InvalidModelState_ReturnsBadRequest()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            controller.ModelState.AddModelError("NotificationSummary", "Required");

            // When
            IActionResult? result = controller.Create(notificationDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void Create_InvalidNotificationType_ReturnsBadRequest()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            notificationTypeRepositoryMock.Setup(r => r.GetTypeByNameAsync(notificationDTO.NotificationTypeName))
                                          .ReturnsAsync((NotificationType?)null);

            // When
            IActionResult? result = controller.Create(notificationDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        #endregion
    }
}
