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

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class FairRandomControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IFairRandomRepository>? fairRandomRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private FairRandomController? controller;

        private User? normalUser;
        private User? otherUser;
        private FairRandom? fairRandom;
        private FairRandomDTO? fairRandomDTO;
        private List<FairRandom>? fairRandomList;
        private List<FairRandomDTO>? fairRandomDTOList;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            fairRandomRepositoryMock = new Mock<IFairRandomRepository>();
            configurationMock = new Mock<IConfiguration>();

            normalUser = UserFixture.GetNormalUser();
            otherUser = UserFixture.GetAdminUser();
            fairRandom = FairRandomFixture.GetFairRandom();
            fairRandomDTO = FairRandomFixture.GetSingleFairRandomDTO();
            fairRandomList = FairRandomFixture.GetFairRandoms();
            fairRandomDTOList = FairRandomFixture.GetFairRandomDTOs();

            controller = new FairRandomController(
                mapperMock.Object,
                fairRandomRepositoryMock.Object,
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
            fairRandomRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<FairRandom>?>()), Times.Never);
        }

        [TestMethod]
        public void GetByUser_AuthenticatedUser_ReturnsFairRandoms()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);

            List<FairRandom> mixedFairRandomList = FairRandomFixture.GetFairRandoms();
            _ = fairRandomRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<FairRandom>?>()))
                                    .ReturnsAsync(mixedFairRandomList);

            _ = mapperMock.Setup(m => m.Map<IEnumerable<FairRandomDTO>>(mixedFairRandomList))
                      .Returns(fairRandomDTOList);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            fairRandomRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<FairRandom>?>()), Times.Once);
        }

        [TestMethod]
        public void GetByUser_AuthenticatedUser_EmptyList_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            var emptyList = new List<FairRandom>();
            _ = fairRandomRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<FairRandom>?>()))
                                    .ReturnsAsync(emptyList);

            _ = mapperMock.Setup(m => m.Map<IEnumerable<FairRandomDTO>>(emptyList))
                      .Returns(new List<FairRandomDTO>());

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            fairRandomRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<FairRandom>?>()), Times.Once);
        }

        #endregion

        #region GetServerHash Tests

        [TestMethod]
        public void GetServerHash_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetServerHash().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            fairRandomRepositoryMock.Verify(r => r.Init(It.IsAny<int>(), true), Times.Never);
        }

        [TestMethod]
        public void GetServerHash_AuthenticatedUser_ReturnsServerHash()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);

            FairRandom fairRandom = FairRandomFixture.GetFairRandom();
            _ = fairRandomRepositoryMock.Setup(r => r.Init(normalUser.UserId, true))
                                    .ReturnsAsync(fairRandom);

            // When
            IActionResult? result = controller.GetServerHash().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(fairRandom.ServerHash, okResult?.Value);
            fairRandomRepositoryMock.Verify(r => r.Init(normalUser.UserId, true), Times.Once);
        }

        #endregion
    }
}
