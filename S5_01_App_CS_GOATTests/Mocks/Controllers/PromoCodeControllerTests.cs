using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.Controllers;
using Shared.DTO;
using Shared.DTO.Helpers;
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
        private Mock<IPromoCodeRepository>? promoCodeRepositoryMock;
        private Mock<IReadableRepository<Case, int>>? caseRepositoryMock;
        private Mock<IMapper>? mapperMock;
        private PromoCodeController? controller;

        private User? admin;
        private User? normalUser;
        private PromoCode? promoCode;
        private PromoCode? expiredPromoCode;
        private PromoCode? userPromoCode;
        private List<PromoCode>? promoCodes;
        private PromoCode? newPromoCode;
        private PromoCode? updatedPromoCode;
        private Case? testCase;
        private Mock<IConfiguration>? configurationMock;

        [TestInitialize]
        public void Initialize()
        {
            promoCodeRepositoryMock = new Mock<IPromoCodeRepository>();
            caseRepositoryMock = new Mock<IReadableRepository<Case, int>>();
            mapperMock = new Mock<IMapper>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            promoCode = PromoCodeFixture.GetPromoCode();
            expiredPromoCode = PromoCodeFixture.GetExpiredPromoCode();
            userPromoCode = PromoCodeFixture.GetUserPromoCode();
            promoCodes = PromoCodeFixture.GetPromoCodes();
            newPromoCode = PromoCodeFixture.GetNewPromoCode();
            updatedPromoCode = PromoCodeFixture.GetUpdatedPromoCode();
            testCase = CaseFixture.GetCase();

            controller = new PromoCodeController(
                mapperMock.Object,
                promoCodeRepositoryMock.Object,
                caseRepositoryMock.Object,
                configurationMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region Check Tests

        [TestMethod]
        public void Check_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.Check("SUMMER2024", null).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            promoCodeRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<PromoCode, bool>>>(), It.IsAny<string[]>()), Times.Never);
        }

        [TestMethod]
        public void Check_ValidPromoCodeWithoutCase_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            string code = "SUMMER2024";
            promoCode.CaseId = null; // No case restriction for this test
            var promoCodeList = new List<PromoCode> { promoCode };
            var expectedDto = new CasePromoCodeDTO
            {
                Code = code,
                DiscountPercentage = 15,
                DiscountAmount = 10.00
            };

            promoCodeRepositoryMock.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<PromoCode, bool>>>(),
                It.IsAny<string[]>()))
                .ReturnsAsync(new List<PromoCode> { promoCode });

            promoCodeRepositoryMock.Setup(r => r.Check(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int?>()
            )).ReturnsAsync(promoCode);

            mapperMock.Setup(m => m.Map<CasePromoCodeDTO>(promoCode))
                .Returns(expectedDto);

            // When
            IActionResult? result = controller.Check(code, null).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(CasePromoCodeDTO));
        }

        [TestMethod]
        public void Check_ValidPromoCodeWithCase_ReturnsOkWithPrices()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            string code = "SUMMER2024";
            int caseId = 1;
            promoCode.CaseId = caseId;
            var promoCodeList = new List<PromoCode> { promoCode };
            var expectedDto = new CasePromoCodeDTO
            {
                Code = code,
                DiscountPercentage = 15,
                DiscountAmount = 10.00,
                CaseId = caseId
            };

            caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId))
                .ReturnsAsync(testCase);

            promoCodeRepositoryMock.Setup(r => r.Check(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int?>()
            )).ReturnsAsync(userPromoCode);

            mapperMock.Setup(m => m.Map<CasePromoCodeDTO>(userPromoCode))
                .Returns(expectedDto);

            // When
            IActionResult? result = controller.Check(code, caseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var dto = okResult.Value as CasePromoCodeDTO;
            Assert.IsNotNull(dto);
            Assert.AreEqual(testCase.CasePrice, dto.BasePrice);
            Assert.IsNotNull(dto.FinalPrice);
        }

        [TestMethod]
        public void Check_PromoCodeNotFound_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            string code = "NONEXISTENT";
            var emptyList = new List<PromoCode>();

            promoCodeRepositoryMock.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<PromoCode, bool>>>(),
                It.IsAny<string[]>()))
                .ReturnsAsync(emptyList);

            // When
            IActionResult? result = controller.Check(code, null).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Check_ExpiredPromoCode_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            string code = "EXPIRED2023";
            var promoCodeList = new List<PromoCode> { expiredPromoCode };

            promoCodeRepositoryMock.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<PromoCode, bool>>>(),
                It.IsAny<string[]>()))
                .ReturnsAsync(promoCodeList);

            // When
            IActionResult? result = controller.Check(code, null).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Check_CaseNotFound_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            string code = "SUMMER2024";
            int invalidCaseId = 999;

            caseRepositoryMock.Setup(r => r.GetByIdAsync(invalidCaseId))
                .ReturnsAsync((Case?)null);

            // When
            IActionResult? result = controller.Check(code, invalidCaseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            promoCodeRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<PromoCode, bool>>>(), It.IsAny<string[]>()), Times.Never);
        }

        [TestMethod]
        public void Check_PromoCodeForDifferentCase_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            string code = "SUMMER2024";
            int requestedCaseId = 2;
            promoCode.CaseId = 1; // Different case
            var promoCodeList = new List<PromoCode> { promoCode };
            var differentCase = new Case { CaseId = requestedCaseId, CaseName = "Different Case", CasePrice = 5.00 };

            caseRepositoryMock.Setup(r => r.GetByIdAsync(requestedCaseId))
                .ReturnsAsync(differentCase);

            promoCodeRepositoryMock.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<PromoCode, bool>>>(),
                It.IsAny<string[]>()))
                .ReturnsAsync(promoCodeList);

            // When
            IActionResult? result = controller.Check(code, requestedCaseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Check_PromoCodeWithNoCaseRestriction_WorksWithAnyCase()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            string code = "USERSPECIAL";
            int caseId = 1;
            userPromoCode.CaseId = null; // No case restriction
            var promoCodeList = new List<PromoCode> { userPromoCode };
            var expectedDto = new CasePromoCodeDTO
            {
                Code = code,
                DiscountPercentage = 10,
                DiscountAmount = 5.00
            };

            caseRepositoryMock.Setup(r => r.GetByIdAsync(caseId))
                .ReturnsAsync(testCase);

            promoCodeRepositoryMock.Setup(r => r.Check(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int?>()
            )).ReturnsAsync(userPromoCode);

            mapperMock.Setup(m => m.Map<CasePromoCodeDTO>(userPromoCode))
                .Returns(expectedDto);

            // When
            IActionResult? result = controller.Check(code, caseId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        #endregion

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
