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
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;
using System.Threading;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class FavoriteControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<Favorite, (int, int)>>? favoriteRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private FavoriteController? controller;

        private User? normalUser;
        private User? otherUser;
        private Favorite? favorite;
        private (int, int) favoriteKey;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            favoriteRepositoryMock = new Mock<IDataRepository<Favorite, (int, int)>>();
            configurationMock = new Mock<IConfiguration>();

            normalUser = UserFixture.GetNormalUser();
            otherUser = UserFixture.GetAdminUser();
            favorite = FavoriteFixture.GetFavorite();

            controller = new FavoriteController(
                mapperMock.Object,
                favoriteRepositoryMock.Object,
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
            // When
            IActionResult? result = controller.Create(favorite).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            favoriteRepositoryMock.Verify(r => r.AddAsync(favorite), Times.Never);
        }

        [TestMethod]
        public void Create_ValidFavorite_ReturnsCreatedAtAction()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            favoriteRepositoryMock.Setup(r => r.AddAsync(favorite))
                                  .ReturnsAsync(favorite);

            // When
            IActionResult? result = controller.Create(favorite).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            favoriteRepositoryMock.Verify(r => r.AddAsync(favorite), Times.Once);
        }

        [TestMethod]
        public void Create_InvalidModelState_ReturnsBadRequest()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            controller.ModelState.AddModelError("UserId", "Required");

            // When
            IActionResult? result = controller.Create(favorite).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            favoriteRepositoryMock.Verify(r => r.AddAsync(favorite), Times.Never);
        }

        [TestMethod]
        public void Create_UserIdMismatch_ReturnsForbid()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            Favorite otherUserFavorite = FavoriteFixture.GetOtherUserFavorite();

            // When
            IActionResult? result = controller.Create(otherUserFavorite).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            favoriteRepositoryMock.Verify(r => r.AddAsync(otherUserFavorite), Times.Never);
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
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey), Times.Never);
            favoriteRepositoryMock.Verify(r => r.DeleteAsync(favorite), Times.Never);
        }

        [TestMethod]
        public void Delete_ExistingFavorite_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int caseId = 1;
            favoriteKey = FavoriteFixture.GetFavoriteKey(normalUser.UserId, caseId);
            
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey))
                                  .ReturnsAsync(favorite);
            favoriteRepositoryMock.Setup(r => r.DeleteAsync(favorite))
                                  .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.Delete(caseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey), Times.Once);
            favoriteRepositoryMock.Verify(r => r.DeleteAsync(favorite), Times.Once);
        }

        [TestMethod]
        public void Delete_NonExistingFavorite_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int caseId = 999;
            favoriteKey = FavoriteFixture.GetFavoriteKey(normalUser.UserId, caseId);
            
            favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteKey))
                                  .ReturnsAsync((Favorite?)null);

            // When
            IActionResult? result = controller.Delete(caseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            favoriteRepositoryMock.Verify(r => r.GetByIdAsync(favoriteKey), Times.Once);
            favoriteRepositoryMock.Verify(r => r.DeleteAsync(favorite), Times.Never);
        }

        #endregion
    }
}
