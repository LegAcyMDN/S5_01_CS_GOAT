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
    public class CaseControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IReadableRepository<Case, int>>? caseRepositoryMock;
        private Mock<IDataRepository<Favorite, (int, int)>>? favoriteRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private Mock<ICaseOpenningRepository>? caseOpenningServiceMock;
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
            caseOpenningServiceMock = new Mock<ICaseOpenningRepository>();
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
                caseOpenningServiceMock.Object,
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
            _ = caseRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Case>?>()))
                              .ReturnsAsync(caseList);
            _ = mapperMock.Setup(m => m.Map<IEnumerable<CaseDTO>>(caseList))
                      .Returns(caseDTOList);

            //When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<Case>?>()), Times.Once);
        }

        [TestMethod]
        public void GetAll_Authenticated_ReturnsOkWithFavorites()
        {
            //Given
            JwtService.AuthentifyController(controller, normalUser);
            _ = caseRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Case>?>()))
                              .ReturnsAsync(caseList);
            _ = mapperMock.Setup(m => m.Map<IEnumerable<CaseDTO>>(caseList))
                      .Returns(caseDTOList);

            _ = favoriteRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Favorite>?>()))
                                  .ReturnsAsync(new List<Favorite>());

            //When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<Case>?>()), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<Favorite>?>()), Times.Once);
        }

        [TestMethod]
        public void GetAll_Authenticated_MarksUserFavoritesCorrectly()
        {
            //Given
            JwtService.AuthentifyController(controller, normalUser);
            _ = caseRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Case>?>()))
                              .ReturnsAsync(caseList);
            _ = mapperMock.Setup(m => m.Map<IEnumerable<CaseDTO>>(caseList))
                      .Returns(caseDTOList);

            _ = favoriteRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Favorite>?>()))
                                  .ReturnsAsync(new List<Favorite> { favorite });

            //When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<Case>?>()), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<Favorite>?>()), Times.Once);
        }

        #endregion

        #region Get Tests

        [TestMethod]
        public void Get_ExistingCase_Unauthenticated_ReturnsOkWithoutFavorite()
        {
            //Given
            int caseId = 1;
            QueryOptions<Case> options = new QueryOptions<Case>().Before(c => c.CaseContents);
            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId, It.IsAny<QueryOptions<Case>>()))
                              .ReturnsAsync(caseEntity);
            _ = mapperMock.Setup(m => m.Map<CaseDTO>(caseEntity))
                      .Returns(caseDTO);

            //When
            IActionResult? result = controller.Get(caseId).GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId, It.IsAny<QueryOptions<Case>>()), Times.Once);
        }

        [TestMethod]
        public void Get_ExistingCase_Authenticated_ReturnsOkWithFavoriteStatus()
        {
            //Given
            JwtService.AuthentifyController(controller, normalUser);
            int caseId = 1;
            favoriteKey = FavoriteFixture.GetFavoriteKey(normalUser.UserId, caseId);

            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId, It.IsAny<QueryOptions<Case>>()))
                              .ReturnsAsync(caseEntity);
            _ = mapperMock.Setup(m => m.Map<CaseDTO>(caseEntity))
                      .Returns(caseDTO);
            _ = favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()))
                                  .ReturnsAsync(favorite);

            //When
            IActionResult? result = controller.Get(caseId).GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId, It.IsAny<QueryOptions<Case>>()), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()), Times.Once);
        }

        [TestMethod]
        public void Get_NonExistingCase_ReturnsNotFound()
        {
            //Given
            int caseId = 999;
            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId, It.IsAny<QueryOptions<Case>>()))
                              .ReturnsAsync((Case?)null);

            //When
            IActionResult? result = controller.Get(caseId).GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId, It.IsAny<QueryOptions<Case>>()), Times.Once);
        }

        [TestMethod]
        public void Get_Authenticated_CaseNotFavorite_ReturnsOkWithFalseFlag()
        {
            //Given
            JwtService.AuthentifyController(controller, normalUser);
            int caseId = 1;
            favoriteKey = FavoriteFixture.GetFavoriteKey(normalUser.UserId, caseId);

            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId, It.IsAny<QueryOptions<Case>>()))
                              .ReturnsAsync(caseEntity);
            _ = mapperMock.Setup(m => m.Map<CaseDTO>(caseEntity))
                      .Returns(caseDTO);
            _ = favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()))
                                  .ReturnsAsync((Favorite?)null);

            //When
            IActionResult? result = controller.Get(caseId).GetAwaiter().GetResult();

            //Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            caseRepositoryMock.Verify(r => r.GetByIdAsync(caseId, It.IsAny<QueryOptions<Case>>()), Times.Once);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()), Times.Once);
        }

        #endregion
    }
}

