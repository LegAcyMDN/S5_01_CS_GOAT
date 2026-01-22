using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class FavoriteControllerTests
    {
        private Mock<IDataRepository<Favorite, (int, int)>>? favoriteRepositoryMock;
        private Mock<IReadableRepository<Case, int>>? caseRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private FavoriteController? controller;

        private User? normalUser;
        private User? otherUser;
        private Favorite? favorite;
        private (int, int) favoriteKey;

        [TestInitialize]
        public void Initialize()
        {
            favoriteRepositoryMock = new Mock<IDataRepository<Favorite, (int, int)>>();
            caseRepositoryMock = new Mock<IReadableRepository<Case, int>>();
            configurationMock = new Mock<IConfiguration>();

            normalUser = UserFixture.GetNormalUser();
            otherUser = UserFixture.GetAdminUser();
            favorite = FavoriteFixture.GetFavorite();

            controller = new FavoriteController(
                favoriteRepositoryMock.Object,
                caseRepositoryMock.Object,
                configurationMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region Create Tests

        [TestMethod]
        public void Create_Unauthenticated_ReturnsUnauthorized()
        {
            // Given
            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(favorite.CaseId, It.IsAny<QueryOptions<Case>>()))
                                  .ReturnsAsync(new Case { CaseId = favorite.CaseId });

            // When
            IActionResult? result = controller.Create(favorite.CaseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            favoriteRepositoryMock.Verify(r => r.AddAsync(favorite), Times.Never);
        }

        [TestMethod]
        public void Create_ValidFavorite_ReturnsCreatedAtAction()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(favorite.CaseId, It.IsAny<QueryOptions<Case>>()))
                                  .ReturnsAsync(new Case { CaseId = favorite.CaseId });
            _ = favoriteRepositoryMock.Setup(r => r.AddAsync(favorite))
                                  .ReturnsAsync(favorite);

            // When
            IActionResult? result = controller.Create(favorite.CaseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            favoriteRepositoryMock.Verify(r => r.PatchAsync(favorite, It.IsAny<Dictionary<string, object>>()),
                Times.Never);
        }

        [TestMethod]
        public void Create_UnknownCase_ReturnsBadRequest()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            controller.ModelState.AddModelError("UserId", "Required");

            // When
            IActionResult? result = controller.Create(favorite.CaseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            favoriteRepositoryMock.Verify(r => r.AddAsync(favorite), Times.Never);
        }

        [TestMethod]
        public void Create_DuplicateFavorite_ReturnsConflict()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            _ = caseRepositoryMock.Setup(r => r.GetByIdAsync(favorite.CaseId, It.IsAny<QueryOptions<Case>?>()))
                                  .ReturnsAsync(new Case { CaseId = favorite.CaseId });
            favoriteKey = FavoriteFixture.GetFavoriteKey(normalUser.UserId, favorite.CaseId);
            _ = favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()))
                                  .ReturnsAsync(favorite);
            // When
            IActionResult? result = controller.Create(favorite.CaseId).GetAwaiter().GetResult();
            // Then
            Assert.IsInstanceOfType(result, typeof(ConflictResult));
            favoriteRepositoryMock.Verify(r => r.AddAsync(favorite), Times.Never);
        }

        #endregion

        #region Delete Tests
        [TestMethod]
        public void Delete_Unauthenticated_ReturnsUnauthorized()
        {
            // Given
            int caseId = 1;

            // When
            IActionResult? result = controller.Delete(caseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            favoriteKey = FavoriteFixture.GetFavoriteKey(2, caseId);
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()), Times.Never);
            favoriteRepositoryMock.Verify(r => r.DeleteAsync(favorite), Times.Never);
        }

        [TestMethod]
        public void Delete_ExistingFavorite_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int caseId = 1;
            favoriteKey = FavoriteFixture.GetFavoriteKey(normalUser.UserId, caseId);

            _ = favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()))
                                  .ReturnsAsync(favorite);
            _ = favoriteRepositoryMock.Setup(r => r.DeleteAsync(favorite))
                                  .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.Delete(caseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()), Times.Once);
            favoriteRepositoryMock.Verify(r => r.DeleteAsync(favorite), Times.Once);
        }

        [TestMethod]
        public void Delete_NonExistingFavorite_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int caseId = 999;
            favoriteKey = FavoriteFixture.GetFavoriteKey(normalUser.UserId, caseId);

            _ = favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()))
                                  .ReturnsAsync((Favorite?)null);

            // When
            IActionResult? result = controller.Delete(caseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey, It.IsAny<QueryOptions<Favorite>?>()), Times.Once);
            favoriteRepositoryMock.Verify(r => r.DeleteAsync(favorite), Times.Never);
        }

        #endregion
    }
}




