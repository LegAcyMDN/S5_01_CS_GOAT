using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Controllers;
using Shared.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class PriceHistoryControllerTests
    {
        private Mock<IPriceHistoryRepository>? priceHistoryRepositoryMock;
        private Mock<IDataRepository<Wear, int>>? wearRepositoryMock;
        private Mock<IMapper>? mapperMock;
        private PriceHistoryController? controller;

        private Wear? wear;
        private List<PriceHistory>? priceHistories;
        private List<PriceHistoryDTO>? priceHistoryDTOs;

        [TestInitialize]
        public void Initialize()
        {
            priceHistoryRepositoryMock = new Mock<IPriceHistoryRepository>();
            wearRepositoryMock = new Mock<IDataRepository<Wear, int>>();
            mapperMock = new Mock<IMapper>();

            wear = WearFixture.GetWear();
            priceHistories = PriceHistoryFixture.GetPriceHistories();
            priceHistoryDTOs = PriceHistoryFixture.GetPriceHistoryDTOs();

            controller = new PriceHistoryController(
                wearRepositoryMock.Object,
                priceHistoryRepositoryMock.Object,
                mapperMock.Object
            );
        }

        #region GetByWear Tests

        [TestMethod]
        public void GetByInventoryItem_ReturnsOk()
        {
            // Given
            wearRepositoryMock.Setup(r => r.GetByIdAsyncNew(wear.WearId, It.IsAny<QueryOptions<Wear>>()))
                                  .ReturnsAsync(wear);
            mapperMock.Setup(m => m.Map<IEnumerable<PriceHistoryDTO>>(priceHistories))
                      .Returns(priceHistoryDTOs);

            // When
            IActionResult? result = controller.GetByWear(wear.WearId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            wearRepositoryMock.Verify(r => r.GetByIdAsyncNew(wear.WearId, It.IsAny<QueryOptions<Wear>>()), Times.Once);
        }

        [TestMethod]
        public void GetByWear_WearNotFound_ReturnsNotFound()
        {
            // Given
            wearRepositoryMock.Setup(r => r.GetByIdAsyncNew(wear.WearId, It.IsAny<QueryOptions<Wear>>()))
                                  .ReturnsAsync((Wear?)null);

            // When
            IActionResult? result = controller.GetByWear(wear.WearId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetByWear_EmptyPriceHistory_ReturnsOkWithEmptyList()
        {
            // Given
            var emptyList = new List<PriceHistory>();
            var emptyDTOList = new List<PriceHistoryDTO>();
            wearRepositoryMock.Setup(r => r.GetByIdAsyncNew(wear.WearId, It.IsAny<QueryOptions<Wear>>()))
                                  .ReturnsAsync(wear);
            mapperMock.Setup(m => m.Map<IEnumerable<PriceHistoryDTO>>(emptyList))
                      .Returns(emptyDTOList);

            // When
            IActionResult? result = controller.GetByWear(wear.WearId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        #endregion

        #region GetAIPrediction Tests

        [TestMethod]
        public void GetAIPrediction_ReturnsOk()
        {
            // Given
            wearRepositoryMock.Setup(r => r.GetByIdAsyncNew(wear.WearId, It.IsAny<QueryOptions<Wear>>()))
                                  .ReturnsAsync(wear);
            priceHistoryRepositoryMock.Setup(r => r.PredictWithAI(wear, 30, false))
                                      .ReturnsAsync(priceHistories);
            mapperMock.Setup(m => m.Map<IEnumerable<PriceHistoryDTO>>(priceHistories))
                      .Returns(priceHistoryDTOs);

            // When
            IActionResult? result = controller.GetAiPrediction(wear.WearId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            priceHistoryRepositoryMock.Verify(r => r.PredictWithAI(wear, It.IsAny<int>(), false), Times.Once);
        }

        [TestMethod]
        public void GetAIPrediction_WearNotFound_ReturnsNotFound()
        {
            // Given
            wearRepositoryMock.Setup(r => r.GetByIdAsyncNew(wear.WearId, It.IsAny<QueryOptions<Wear>>()))
                                  .ReturnsAsync((Wear)null);

            // When
            IActionResult? result = controller.GetAiPrediction(wear.WearId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            wearRepositoryMock.Verify(r => r.GetByIdAsyncNew(wear.WearId, It.IsAny<QueryOptions<Wear>>()), Times.Once);
        }

        #endregion
    }
}


