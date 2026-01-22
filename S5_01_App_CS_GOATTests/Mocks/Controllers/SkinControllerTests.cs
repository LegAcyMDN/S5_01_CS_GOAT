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
    public class SkinControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IReadableRepository<Case, int>>? caseRepositoryMock;
        private SkinController? controller;

        private Case? caseWithSkins;
        private List<SkinDTO>? skinDTOs;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            caseRepositoryMock = new Mock<IReadableRepository<Case, int>>();

            caseWithSkins = SkinFixture.GetCaseWithSkins();
            skinDTOs = SkinFixture.GetSkinDTOs();

            var skinRepositoryMock = new Mock<IReadableRepository<Skin, int>>();
            controller = new SkinController(
                mapperMock.Object,
                caseRepositoryMock.Object,
                skinRepositoryMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region GET Tests

        [TestMethod]
        public void GetByCase_ValidCaseId_ReturnsOk()
        {
            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(
                1,
                It.IsAny<QueryOptions<Case>>()
            )).ReturnsAsync(caseWithSkins);

            var caseContents = caseWithSkins.CaseContents.ToList();
            _ = mapperMock.Setup(m => m.Map<SkinDTO>(caseContents[0].Skin))
                       .Returns(skinDTOs[0]);
            _ = mapperMock.Setup(m => m.Map<SkinDTO>(caseContents[1].Skin))
                       .Returns(skinDTOs[1]);

            // When
            IActionResult? result = controller.GetByCase(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.IsNotNull(okResult.Value);
            caseRepositoryMock.Verify(r => r.GetByIdAsync(
                1,
                It.IsAny<QueryOptions<Case>>()
            ), Times.Once);
        }

        [TestMethod]
        public void GetByCase_InvalidCaseId_ReturnsNotFound()
        {
            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(
                999,
                It.IsAny<QueryOptions<Case>>()
            )).ReturnsAsync((Case?)null);

            // When
            IActionResult? result = controller.GetByCase(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(
                999,
                It.IsAny<QueryOptions<Case>>()
            ), Times.Once);
        }

        [TestMethod]
        public void GetByCase_NegativeCaseId_ReturnsNotFound()
        {
            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(
                -1,
                It.IsAny<QueryOptions<Case>>()
            )).ReturnsAsync((Case?)null);

            // When
            IActionResult? result = controller.GetByCase(-1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetByCase_ZeroCaseId_ReturnsNotFound()
        {
            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(
                0,
                It.IsAny<QueryOptions<Case>>()
            )).ReturnsAsync((Case?)null);

            // When
            IActionResult? result = controller.GetByCase(0).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetByCase_CaseWithEmptySkins_ReturnsOkWithEmptyList()
        {
            var emptyCaseContents = new List<CaseContent>();
            var caseWithNoSkins = new Case
            {
                CaseId = 1,
                CaseName = "Empty Case",
                CaseContents = emptyCaseContents
            };

            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(
                1,
                It.IsAny<QueryOptions<Case>>()
            )).ReturnsAsync(caseWithNoSkins);

            // When
            IActionResult? result = controller.GetByCase(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        #endregion
    }
}

