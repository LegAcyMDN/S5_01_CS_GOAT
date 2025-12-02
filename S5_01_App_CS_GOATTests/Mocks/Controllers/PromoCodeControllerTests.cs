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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class PromoCodeControllerTests
    {
        private Mock<IDataRepository<PromoCode, int>>? promoCodeRepositoryMock;
        private PromoCodeController? controller;

        private User? admin;
        private User? normalUser;
        private PromoCode? promoCode;
        private List<PromoCode>? promoCodes;
        private PromoCode? newPromoCode;
        private PromoCode? updatedPromoCode;
        private Mock<IConfiguration>? configurationMock;

        [TestInitialize]
        public void Initialize()
        {
            promoCodeRepositoryMock = new Mock<IDataRepository<PromoCode, int>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            promoCode = PromoCodeFixture.GetPromoCode();
            promoCodes = PromoCodeFixture.GetPromoCodes();
            newPromoCode = PromoCodeFixture.GetNewPromoCode();
            updatedPromoCode = PromoCodeFixture.GetUpdatedPromoCode();

            controller = new PromoCodeController(
                promoCodeRepositoryMock.Object,
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
        public void GetAll_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            promoCodeRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetAll_AsNonAdmin_ReturnsForbidden()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            promoCodeRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetAll_AsAdmin_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            promoCodeRepositoryMock.Setup(r => r.GetAllAsync(null))
                                   .ReturnsAsync(promoCodes);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            promoCodeRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion

        #region Create Tests

        [TestMethod]
        public void Create_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.Create(newPromoCode).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            promoCodeRepositoryMock.Verify(r => r.AddAsync(newPromoCode), Times.Never);
        }

        [TestMethod]
        public void Create_AsNonAdmin_ReturnsForbidden()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);

            // When
            IActionResult? result = controller.Create(newPromoCode).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            promoCodeRepositoryMock.Verify(r => r.AddAsync(newPromoCode), Times.Never);
        }

        [TestMethod]
        public void Create_AsAdmin_ReturnsCreated()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            promoCodeRepositoryMock.Setup(r => r.AddAsync(newPromoCode))
                                   .ReturnsAsync(newPromoCode);

            // When
            IActionResult? result = controller.Create(newPromoCode).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            promoCodeRepositoryMock.Verify(r => r.AddAsync(newPromoCode), Times.Once);
        }

        [TestMethod]
        public void Create_InvalidModelState_ReturnsBadRequest()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            controller.ModelState.AddModelError("Code", "Required");

            // When
            IActionResult? result = controller.Create(newPromoCode).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            promoCodeRepositoryMock.Verify(r => r.AddAsync(newPromoCode), Times.Never);
        }

        #endregion

        #region Update Tests

        [TestMethod]
        public void Update_Unauthenticated_ReturnsUnauthorized()
        {
            // Given
            int promoCodeId = 1;

            // When
            IActionResult? result = controller.Update(promoCodeId, updatedPromoCode).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            promoCodeRepositoryMock.Verify(r => r.GetByIdAsync(promoCodeId), Times.Never);
        }

        [TestMethod]
        public void Update_AsNonAdmin_ReturnsForbidden()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int promoCodeId = 1;

            // When
            IActionResult? result = controller.Update(promoCodeId, updatedPromoCode).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            promoCodeRepositoryMock.Verify(r => r.GetByIdAsync(promoCodeId), Times.Never);
        }

        [TestMethod]
        public void Update_NonExistingPromoCode_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            int promoCodeId = 999;
            promoCodeRepositoryMock.Setup(r => r.GetByIdAsync(promoCodeId))
                                   .ReturnsAsync((PromoCode?)null);

            // When
            IActionResult? result = controller.Update(promoCodeId, updatedPromoCode).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            promoCodeRepositoryMock.Verify(r => r.GetByIdAsync(promoCodeId), Times.Once);
            promoCodeRepositoryMock.Verify(r => r.UpdateAsync(promoCode, updatedPromoCode), Times.Never);
        }

        [TestMethod]
        public void Update_AsAdmin_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            int promoCodeId = 1;
            promoCodeRepositoryMock.Setup(r => r.GetByIdAsync(promoCodeId))
                                   .ReturnsAsync(promoCode);
            promoCodeRepositoryMock.Setup(r => r.UpdateAsync(promoCode, updatedPromoCode))
                                   .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.Update(promoCodeId, updatedPromoCode).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            promoCodeRepositoryMock.Verify(r => r.GetByIdAsync(promoCodeId), Times.Once);
            promoCodeRepositoryMock.Verify(r => r.UpdateAsync(promoCode, updatedPromoCode), Times.Once);
        }

        [TestMethod]
        public void Update_InvalidModelState_ReturnsBadRequest()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            int promoCodeId = 1;
            controller.ModelState.AddModelError("Code", "Required");

            // When
            IActionResult? result = controller.Update(promoCodeId, updatedPromoCode).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            promoCodeRepositoryMock.Verify(r => r.GetByIdAsync(promoCodeId), Times.Never);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        public void Delete_Unauthenticated_ReturnsUnauthorized()
        {
            // Given
            int promoCodeId = 1;

            // When
            IActionResult? result = controller.Delete(promoCodeId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            promoCodeRepositoryMock.Verify(r => r.GetByIdAsync(promoCodeId), Times.Never);
        }

        [TestMethod]
        public void Delete_AsNonAdmin_ReturnsForbidden()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int promoCodeId = 1;

            // When
            IActionResult? result = controller.Delete(promoCodeId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            promoCodeRepositoryMock.Verify(r => r.GetByIdAsync(promoCodeId), Times.Never);
        }

        [TestMethod]
        public void Delete_NonExistingPromoCode_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            int promoCodeId = 999;
            promoCodeRepositoryMock.Setup(r => r.GetByIdAsync(promoCodeId))
                                   .ReturnsAsync((PromoCode?)null);

            // When
            IActionResult? result = controller.Delete(promoCodeId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            promoCodeRepositoryMock.Verify(r => r.GetByIdAsync(promoCodeId), Times.Once);
            promoCodeRepositoryMock.Verify(r => r.DeleteAsync(promoCode), Times.Never);
        }

        [TestMethod]
        public void Delete_AsAdmin_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            int promoCodeId = 1;
            promoCodeRepositoryMock.Setup(r => r.GetByIdAsync(promoCodeId))
                                   .ReturnsAsync(promoCode);
            promoCodeRepositoryMock.Setup(r => r.DeleteAsync(promoCode))
                                   .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.Delete(promoCodeId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            promoCodeRepositoryMock.Verify(r => r.GetByIdAsync(promoCodeId), Times.Once);
            promoCodeRepositoryMock.Verify(r => r.DeleteAsync(promoCode), Times.Once);
        }

        #endregion
    }
}
