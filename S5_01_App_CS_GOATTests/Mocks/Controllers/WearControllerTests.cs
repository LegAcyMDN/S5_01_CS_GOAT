using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOATTests.Fixtures;
using System.Threading;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class WearControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IReadableRepository<Wear, int>>? wearRepositoryMock;
        private WearController? controller;

        private Wear? wear;
        private ModelDTO? modelDTO;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            wearRepositoryMock = new Mock<IReadableRepository<Wear, int>>();

            wear = WearFixture.GetWearWithFullData();
            modelDTO = WearFixture.GetModelDTO();

            controller = new WearController(
                mapperMock.Object,
                wearRepositoryMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region GET Tests

        [TestMethod]
        public void Get3dModelByWear_ValidWearId_ReturnsOk()
        {
            wearRepositoryMock.Setup(r => r.GetByIdAsync(1, "Skin.Item"))
                               .ReturnsAsync(wear);
            mapperMock.Setup(m => m.Map<ModelDTO>(wear))
                       .Returns(modelDTO);

            // When
            IActionResult? result = controller.Get3dModelByWear(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            Assert.IsNotNull(okResult.Value);
            wearRepositoryMock.Verify(r => r.GetByIdAsync(1, "Skin.Item"), Times.Once);
        }

        [TestMethod]
        public void Get3dModelByWear_InvalidWearId_ReturnsNotFound()
        {
            wearRepositoryMock.Setup(r => r.GetByIdAsync(999, "Skin.Item"))
                               .ReturnsAsync((Wear?)null);

            // When
            IActionResult? result = controller.Get3dModelByWear(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            wearRepositoryMock.Verify(r => r.GetByIdAsync(999, "Skin.Item"), Times.Once);
        }

        #endregion
    }
}
