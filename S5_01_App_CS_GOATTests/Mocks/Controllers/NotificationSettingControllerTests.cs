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
using Shared.DTO;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class NotificationSettingControllerTests
    {
        private Mock<IDataRepository<NotificationSetting, (int, int)>>? notificationSettingRepositoryMock;
        private Mock<ITypeRepository<NotificationType>>? typeRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private Mock<AutoMapper.IMapper>? mapperMock;
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
            typeRepositoryMock = new Mock<ITypeRepository<NotificationType>>();
            configurationMock = new Mock<IConfiguration>();
            mapperMock = new Mock<AutoMapper.IMapper>();

            normalUser = UserFixture.GetNormalUser();
            notificationSetting = NotificationFixture.GetNotificationSetting();
            notificationSettings = NotificationFixture.GetNotificationSettings();
            patchData = NotificationFixture.GetNotificationSettingPatchData();

            notificationSettingKey = NotificationFixture.GetNotificationSettingKeyForNormalUser();

            controller = new NotificationSettingController(
                notificationSettingRepositoryMock.Object,
                typeRepositoryMock.Object,
                configurationMock.Object,
                mapperMock.Object
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
            notificationSettingRepositoryMock.Verify(r => r.GetAllAsyncOld(null), Times.Never);
        }

        [TestMethod]
        public void GetByUser_Authenticated_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            notificationSettingRepositoryMock.Setup(r => r.GetAllAsyncOld(null, "NotificationType"))
                                             .ReturnsAsync(notificationSettings);
            mapperMock.Setup(m => m.Map<IEnumerable<NotificationSettingDTO>>(notificationSettings))
                      .Returns(new List<NotificationSettingDTO>
                      {
                          new NotificationSettingDTO { NotificationTypeName = "Type1", OnSite = true },
                          new NotificationSettingDTO { NotificationTypeName = "Type2", OnSite = false }
                      });

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            notificationSettingRepositoryMock.Verify(r => r.GetAllAsyncOld(null, "NotificationType"), Times.Once);
        }

        #endregion

        #region Update Tests

        [TestMethod]
        public void Update_Unauthenticated_ReturnsUnauthorized()
        {
            // Given
            string notificationTypeName = "CaseOpened";

            // When
            IActionResult? result = controller.Update(notificationTypeName, patchData).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void Update_ValidSetting_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            string notificationTypeName = "CaseOpened";
            int notificationTypeId = 1;
            var notificationType = new NotificationType { NotificationTypeId = notificationTypeId, NotificationTypeName = notificationTypeName };

            typeRepositoryMock.Setup(r => r.GetTypeByName(notificationTypeName))
                             .Returns(notificationType);
            var key = (normalUser.UserId, notificationTypeId);
            notificationSettingRepositoryMock.Setup(r => r.GetByIdAsync(key))
                                             .ReturnsAsync(notificationSetting);
            notificationSettingRepositoryMock.Setup(r => r.PatchAsync(notificationSetting, patchData))
                                             .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.Update(notificationTypeName, patchData).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public void Update_NonExistingSetting_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            string notificationTypeName = "Unknown";

            typeRepositoryMock.Setup(r => r.GetTypeByName(notificationTypeName))
                             .Returns((NotificationType?)null);

            // When
            IActionResult? result = controller.Update(notificationTypeName, patchData).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetByUser_AuthenticatedWithEmptySettings_ReturnsOkWithEmptyList()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            var emptyList = new List<NotificationSetting>();
            notificationSettingRepositoryMock.Setup(r => r.GetAllAsyncOld(null, "NotificationType"))
                                             .ReturnsAsync(emptyList);
            mapperMock.Setup(m => m.Map<IEnumerable<NotificationSettingDTO>>(emptyList))
                      .Returns(new List<NotificationSettingDTO>());

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        #endregion
    }
}


