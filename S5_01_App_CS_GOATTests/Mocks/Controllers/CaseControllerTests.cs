using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;
using System.Threading;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class CaseControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IReadableRepository<Case, int>>? caseRepositoryMock;
        private Mock<IDataRepository<Favorite, (int, int)>>? favoriteRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private CaseController? controller;

        private User? normalUser;
        private Case? caseEntity;
        private CaseDTO? caseDTO;
        private List<Case>? caseList;
        private List<CaseDTO>? caseDTOList;
        private Favorite? favorite;

        private (int, int) favoriteKey1;
        private (int, int) favoriteKey2;
        private (int, int) favoriteKey3;
        private (int, int) favoriteKey;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            caseRepositoryMock = new Mock<IReadableRepository<Case, int>>();
            favoriteRepositoryMock = new Mock<IDataRepository<Favorite, (int, int)>>();
            configurationMock = new Mock<IConfiguration>();

            normalUser = UserFixture.GetNormalUser();
            caseEntity = CaseFixture.GetCase();
            caseDTO = CaseFixture.GetSingleCaseDTO();
            caseList = CaseFixture.GetCases();
            caseDTOList = CaseFixture.GetCaseDTOs();
            favorite = FavoriteFixture.GetFavorite();

            favoriteKey1 = FavoriteFixture.GetFavoriteKey1ForNormalUser();
            favoriteKey2 = FavoriteFixture.GetFavoriteKey2ForNormalUser();
            favoriteKey3 = FavoriteFixture.GetFavoriteKey3ForNormalUser();

            controller = new CaseController(
                mapperMock.Object,
                caseRepositoryMock.Object,
                favoriteRepositoryMock.Object,
                configurationMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region GetAll Tests

        [TestMethod]
        public void GetAll_Unauthenticated_ReturnsOkWithoutFavorites()
        {
            //Given
            caseRepositoryMock.Setup(r => r.GetAllAsync(null))
                              .ReturnsAsync(caseList);
            mapperMock.Setup(m => m.Map<IEnumerable<CaseDTO>>(caseList))
                      .Returns(caseDTOList);

            //When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        [TestMethod]
        public void GetAll_Authenticated_ReturnsOkWithFavorites()
        {
            //Given
            JwtService.AuthentifyController(controller, normalUser);
            caseRepositoryMock.Setup(r => r.GetAllAsync(null))
                              .ReturnsAsync(caseList);
            mapperMock.Setup(m => m.Map<IEnumerable<CaseDTO>>(caseList))
                      .Returns(caseDTOList);
            
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey1))
                                  .ReturnsAsync((Favorite?)null);
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey2))
                                  .ReturnsAsync((Favorite?)null);
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey3))
                                  .ReturnsAsync((Favorite?)null);

            //When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey1), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey2), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey3), Times.Once);
        }

        [TestMethod]
        public void GetAll_Authenticated_MarksUserFavoritesCorrectly()
        {
            //Given
            JwtService.AuthentifyController(controller, normalUser);
            caseRepositoryMock.Setup(r => r.GetAllAsync(null))
                              .ReturnsAsync(caseList);
            mapperMock.Setup(m => m.Map<IEnumerable<CaseDTO>>(caseList))
                      .Returns(caseDTOList);
            
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey1))
                                  .ReturnsAsync(favorite);
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey2))
                                  .ReturnsAsync((Favorite?)null);
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey3))
                                  .ReturnsAsync((Favorite?)null);

            //When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey1), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey2), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey3), Times.Once);
        }

        #endregion

        #region Get Tests

        [TestMethod]
        public void Get_ExistingCase_Unauthenticated_ReturnsOkWithoutFavorite()
        {
            //Given
            int caseId = 1;
            caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId, "CaseContents"))
                              .ReturnsAsync(caseEntity);
            mapperMock.Setup(m => m.Map<CaseDTO>(caseEntity))
                      .Returns(caseDTO);

            //When
            IActionResult? result = controller.Get(caseId).GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId, "CaseContents"), Times.Once);
        }

        [TestMethod]
        public void Get_ExistingCase_Authenticated_ReturnsOkWithFavoriteStatus()
        {
            //Given
            JwtService.AuthentifyController(controller, normalUser);
            int caseId = 1;
            favoriteKey = FavoriteFixture.GetFavoriteKey(normalUser.UserId, caseId);
            
            caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId, "CaseContents"))
                              .ReturnsAsync(caseEntity);
            mapperMock.Setup(m => m.Map<CaseDTO>(caseEntity))
                      .Returns(caseDTO);
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey))
                                  .ReturnsAsync(favorite);

            //When
            IActionResult? result = controller.Get(caseId).GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId, "CaseContents"), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey), Times.Once);
        }

        [TestMethod]
        public void Get_NonExistingCase_ReturnsNotFound()
        {
            //Given
            int caseId = 999;
            caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId, "CaseContents"))
                              .ReturnsAsync((Case?)null);

            //When
            IActionResult? result = controller.Get(caseId).GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId, "CaseContents"), Times.Once);
        }

        [TestMethod]
        public void Get_Authenticated_CaseNotFavorite_ReturnsOkWithFalseFlag()
        {
            //Given
            JwtService.AuthentifyController(controller, normalUser);
            int caseId = 1;
            favoriteKey = FavoriteFixture.GetFavoriteKey(normalUser.UserId, caseId);
            
            caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId, "CaseContents"))
                              .ReturnsAsync(caseEntity);
            mapperMock.Setup(m => m.Map<CaseDTO>(caseEntity))
                      .Returns(caseDTO);
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey))
                                  .ReturnsAsync((Favorite?)null);

            //When
            IActionResult? result = controller.Get(caseId).GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId, "CaseContents"), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey), Times.Once);
        }

        #endregion
    }
}
