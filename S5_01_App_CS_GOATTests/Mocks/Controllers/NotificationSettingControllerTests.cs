using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class NotificationSettingControllerTests
    {
        private Mock<IDataRepository<NotificationSetting, (int, int)>>? notificationSettingRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private NotificationSettingController? controller;

        private User? normalUser;
        private NotificationSetting? notificationSetting;
        private List<NotificationSetting>? notificationSettings;
        private Dictionary<string, object>? patchData;

        private (int, int) notificationSettingKey;

        [TestInitialize]
        public void Initialize()
        {
            notificationSettingRepositoryMock = new Mock<IDataRepository<NotificationSetting, (int, int)>>();
            configurationMock = new Mock<IConfiguration>();

            normalUser = UserFixture.GetNormalUser();
            notificationSetting = NotificationFixture.GetNotificationSetting();
            notificationSettings = NotificationFixture.GetNotificationSettings();
            patchData = NotificationFixture.GetNotificationSettingPatchData();

            notificationSettingKey = NotificationFixture.GetNotificationSettingKeyForNormalUser();

            controller = new NotificationSettingController(
                notificationSettingRepositoryMock.Object,
                configurationMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region GetByUser Tests

        [TestMethod]
        public void GetByUser_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            notificationSettingRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetByUser_Authenticated_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            notificationSettingRepositoryMock.Setup(r => r.GetAllAsync(null))
                                             .ReturnsAsync(notificationSettings);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            notificationSettingRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion

        #region Update Tests

        [TestMethod]
        public void Update_Unauthenticated_ReturnsUnauthorized()
        {
            // Given
            int notificationTypeId = 1;

            // When
            IActionResult? result = controller.Update(notificationTypeId, patchData).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            notificationSettingRepositoryMock.Verify(r => r.GetByIdAsync(notificationSettingKey), Times.Never);
        }

        [TestMethod]
        public void Update_ValidSetting_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int notificationTypeId = 1;

            notificationSettingRepositoryMock.Setup(r => r.GetByIdAsync(notificationSettingKey))
                                             .ReturnsAsync(notificationSetting);
            notificationSettingRepositoryMock.Setup(r => r.PatchAsync(notificationSetting, patchData))
                                             .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.Update(notificationTypeId, patchData).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            notificationSettingRepositoryMock.Verify(r => r.GetByIdAsync(notificationSettingKey), Times.Once);
        }

        [TestMethod]
        public void Update_NonExistingSetting_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int notificationTypeId = 999;
            (int, int) nonExistingKey = NotificationFixture.GetNotificationSettingKey(normalUser.UserId, notificationTypeId);

            notificationSettingRepositoryMock.Setup(r => r.GetByIdAsync(nonExistingKey))
                                             .ReturnsAsync((NotificationSetting?)null);

            // When
            IActionResult? result = controller.Update(notificationTypeId, patchData).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            notificationSettingRepositoryMock.Verify(r => r.GetByIdAsync(nonExistingKey), Times.Once);
        }

        #endregion
    }
}
