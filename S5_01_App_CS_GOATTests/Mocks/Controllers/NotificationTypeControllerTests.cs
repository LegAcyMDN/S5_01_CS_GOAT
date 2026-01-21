using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;
using Shared.DTO;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class NotificationTypeControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<ITypeRepository<NotificationType>>? notificationTypeRepositoryMock;
        private NotificationTypeController? controller;

        private List<NotificationType>? notificationTypes;
        private List<NotificationTypeDTO>? notificationTypeDTOs;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            notificationTypeRepositoryMock = new Mock<ITypeRepository<NotificationType>>();

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
            _ = notificationTypeRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<NotificationType>?>()))
                                          .ReturnsAsync(notificationTypes);
            _ = mapperMock.Setup(m => m.Map<IEnumerable<NotificationTypeDTO>>(notificationTypes))
                      .Returns(notificationTypeDTOs);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            notificationTypeRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<NotificationType>?>()), Times.Once);
        }

        [TestMethod]
        public void GetAll_EmptyList_ReturnsOkWithEmptyList()
        {
            // Given
            var emptyList = new List<NotificationType>();
            var emptyDTOList = new List<NotificationTypeDTO>();
            _ = notificationTypeRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<NotificationType>?>()))
                                          .ReturnsAsync(emptyList);
            _ = mapperMock.Setup(m => m.Map<IEnumerable<NotificationTypeDTO>>(emptyList))
                      .Returns(emptyDTOList);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult?)result;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public void GetAll_RepositoryThrowsException_ReturnsOkWithEmptyResult()
        {
            // Given
            _ = notificationTypeRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<NotificationType>?>()))
                                          .ThrowsAsync(new Exception("Database error"));

            // When & Then
            try
            {
                _ = controller.GetAll().GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Database error"));
            }
        }

        [TestMethod]
        public void GetAll_MappingFails_StillReturnsOk()
        {
            // Given
            _ = notificationTypeRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<NotificationType>?>()))
                                          .ReturnsAsync(notificationTypes);
            _ = mapperMock.Setup(m => m.Map<IEnumerable<NotificationTypeDTO>>(notificationTypes))
                      .Throws(new AutoMapperMappingException("Mapping failed"));

            // When & Then
            try
            {
                _ = controller.GetAll().GetAwaiter().GetResult();
                Assert.Fail("Expected exception was not thrown");
            }
            catch (AutoMapperMappingException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Mapping failed"));
            }
        }

        #endregion
    }
}

