using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOATTests.Fixtures;
using System.Collections.Generic;
using System.Threading;

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

            controller = new SkinController(
                mapperMock.Object,
                caseRepositoryMock.Object
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
            caseRepositoryMock.Setup(r => r.GetByIdAsync(
                1,
              "CaseContents.Skin.Rarity", "CaseContents.Skin.PriceHistories.WearType", "CaseContents.Skin.Wears.WearType"
            )).ReturnsAsync(caseWithSkins);
            
            List<CaseContent> caseContents = caseWithSkins.CaseContents.ToList();
            mapperMock.Setup(m => m.Map<SkinDTO>(caseContents[0].Skin))
                       .Returns(skinDTOs[0]);
            mapperMock.Setup(m => m.Map<SkinDTO>(caseContents[1].Skin))
                       .Returns(skinDTOs[1]);

            // When
            IActionResult? result = controller.GetByCase(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            Assert.IsNotNull(okResult.Value);
            caseRepositoryMock.Verify(r => r.GetByIdAsync(
                1,
               "CaseContents.Skin.Rarity", "CaseContents.Skin.PriceHistories.WearType", "CaseContents.Skin.Wears.WearType"
            ), Times.Once);
        }

        [TestMethod]
        public void GetByCase_InvalidCaseId_ReturnsNotFound()
        {
            caseRepositoryMock.Setup(r => r.GetByIdAsync(
                999,
             "CaseContents.Skin.Rarity", "CaseContents.Skin.PriceHistories.WearType", "CaseContents.Skin.Wears.WearType"
            )).ReturnsAsync((Case?)null);

            // When
            IActionResult? result = controller.GetByCase(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(
                999,
              "CaseContents.Skin.Rarity", "CaseContents.Skin.PriceHistories.WearType", "CaseContents.Skin.Wears.WearType"
            ), Times.Once);
        }

        #endregion
    }
}
