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
    public class PriceHistoryControllerTests
    {
        private Mock<IDataRepository<PriceHistory, int>>? priceHistoryRepositoryMock;
        private Mock<IMapper>? mapperMock;
        private PriceHistoryController? controller;

        private List<PriceHistory>? priceHistories;
        private List<PriceHistoryDTO>? priceHistoryDTOs;

        [TestInitialize]
        public void Initialize()
        {
            priceHistoryRepositoryMock = new Mock<IDataRepository<PriceHistory, int>>();
            mapperMock = new Mock<IMapper>();

            priceHistories = PriceHistoryFixture.GetPriceHistories();
            priceHistoryDTOs = PriceHistoryFixture.GetPriceHistoryDTOs();

            controller = new PriceHistoryController(
                priceHistoryRepositoryMock.Object,
                mapperMock.Object
            );
        }

        #region GetByInventoryItem Tests

        [TestMethod]
        public void GetByInventoryItem_ReturnsOk()
        {
            // Given
            int wearId = 1;
            priceHistoryRepositoryMock.Setup(r => r.GetAllAsync(ph => ph.WearId == wearId))
                                      .ReturnsAsync(priceHistories);
            mapperMock.Setup(m => m.Map<IEnumerable<PriceHistoryDTO>>(priceHistories))
                      .Returns(priceHistoryDTOs);

            // When
            IActionResult? result = controller.GetByInventoryItem(wearId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            priceHistoryRepositoryMock.Verify(r => r.GetAllAsync(ph => ph.WearId == wearId), Times.Once);
        }

        #endregion
    }
}
