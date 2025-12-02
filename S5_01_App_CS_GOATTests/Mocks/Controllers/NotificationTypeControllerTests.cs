using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOATTests.Fixtures;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class NotificationTypeControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<NotificationType, int>>? notificationTypeRepositoryMock;
        private NotificationTypeController? controller;

        private List<NotificationType>? notificationTypes;
        private List<NotificationTypeDTO>? notificationTypeDTOs;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            notificationTypeRepositoryMock = new Mock<IDataRepository<NotificationType, int>>();

            notificationTypes = NotificationFixture.GetNotificationTypes();
            notificationTypeDTOs = NotificationFixture.GetNotificationTypeDTOs();

            controller = new NotificationTypeController(
                mapperMock.Object,
                notificationTypeRepositoryMock.Object
            );
        }

        #region GetAll Tests

        [TestMethod]
        public void GetAll_ReturnsOk()
        {
            // Given
            notificationTypeRepositoryMock.Setup(r => r.GetAllAsync(null))
                                          .ReturnsAsync(notificationTypes);
            mapperMock.Setup(m => m.Map<IEnumerable<NotificationTypeDTO>>(notificationTypes))
                      .Returns(notificationTypeDTOs);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            notificationTypeRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion
    }
}
