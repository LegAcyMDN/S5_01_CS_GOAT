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
    public class BanControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<Ban, int>>? banRepositoryMock;
        private Mock<ITypeRepository<BanType>> banTypeRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private BanController? controller;

        private User? admin;
        private User? normalUser;
        private Ban? ban;
        private BanDTO? banDTO;
        private BanType? banType;
        private List<Ban>? banList;
        private List<BanDTO>? banDTOList;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            banRepositoryMock = new Mock<IDataRepository<Ban, int>>();
            banTypeRepositoryMock = new Mock<ITypeRepository<BanType>>();
            configurationMock = new Mock<IConfiguration>();

            admin = TestFixture.GetAdminUser();
            normalUser = TestFixture.GetNormalUser();
            banType = TestFixture.GetBanType();
            banDTO = TestFixture.GetSingleBanDTO();
            banDTOList = TestFixture.GetBanDTOs();
            ban = TestFixture.GetBan();
            banList = TestFixture.GetBans();

            controller = new BanController(
                mapperMock.Object,
                banRepositoryMock.Object,
                banTypeRepositoryMock.Object,
                configurationMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region GET Tests

        [TestMethod]
        public void GetAll_AsAdmin_ReturnsOk()
        {
            JwtService.AuthentifyController(controller, admin);
            banRepositoryMock.Setup(r => r.GetAllAsync(null, It.IsAny<string[]>())).ReturnsAsync(banList);
            mapperMock.Setup(m => m.Map<IEnumerable<BanDTO>>(banList))
                       .Returns(banDTOList);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            banRepositoryMock.Verify(r => r.GetAllAsync(null, It.IsAny<string[]>()), Times.Once);
        }

        [TestMethod]
        public void GetAll_AsNonAdmin_ReturnsForbidden()
        {
            JwtService.AuthentifyController(controller, normalUser);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public void GetByUser_AuthenticatedUser_ReturnsOwnBans()
        {
            JwtService.AuthentifyController(controller, normalUser);
            
            banRepositoryMock.Setup(r => r.GetAllAsync(null, It.IsAny<string[]>())).ReturnsAsync(banList);
            mapperMock.Setup(m => m.Map<IEnumerable<BanDTO>>(banList))
                       .Returns(banDTOList);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            banRepositoryMock.Verify(r => r.GetAllAsync(null, It.IsAny<string[]>()), Times.Once);
        }

        [TestMethod]
        public void GetByUser_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            banRepositoryMock.Verify(r => r.GetAllAsync(null, It.IsAny<string[]>()), Times.Never);
        }

        #endregion

        #region POST Tests

        [TestMethod]
        public void Create_ValidBan_ReturnsCreatedAtAction()
        {
            JwtService.AuthentifyController(controller, admin);
            banTypeRepositoryMock.Setup(r => r.GetTypeByNameAsync(banDTO.BanTypeName))
                                   .ReturnsAsync(banType);
            mapperMock.Setup(m => m.Map<Ban>(banDTO)).Returns(ban);
            banRepositoryMock.Setup(r => r.AddAsync(ban)).ReturnsAsync(ban);
            mapperMock.Setup(m => m.Map<BanDTO>(ban)).Returns(banDTO);

            // When
            IActionResult? result = controller.Create(banDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            banRepositoryMock.Verify(r => r.AddAsync(ban), Times.Once);
        }

        [TestMethod]
        public void Create_InvalidModel_ReturnsBadRequest()
        {
            JwtService.AuthentifyController(controller, admin);
            controller.ModelState.AddModelError("BanReason", "Required");
            BanDTO emptyBanDTO = TestFixture.GetEmptyBanDTO();

            // When
            IActionResult? result = controller.Create(emptyBanDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            banRepositoryMock.Verify(r => r.AddAsync(ban), Times.Never);
        }

        [TestMethod]
        public void Create_InvalidBanType_ReturnsBadRequest()
        {
            JwtService.AuthentifyController(controller, admin);
            banTypeRepositoryMock.Setup(r => r.GetTypeByNameAsync(banDTO.BanTypeName))
                                   .ReturnsAsync((BanType?)null);

            // When
            IActionResult? result = controller.Create(banDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            banRepositoryMock.Verify(r => r.AddAsync(ban), Times.Never);
        }

        #endregion

        #region PUT Tests

        [TestMethod]
        public void Update_ValidBan_ReturnsNoContent()
        {
            JwtService.AuthentifyController(controller, admin);
            Ban updatedBan = TestFixture.GetUpdatedBan();

            banRepositoryMock.Setup(r => r.GetByIdAsync(ban.UserId))
                              .ReturnsAsync(ban);
            mapperMock.Setup(m => m.Map<Ban>(banDTO))
                       .Returns(updatedBan);
            banRepositoryMock.Setup(r => r.UpdateAsync(ban, updatedBan))
                              .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.Update(ban.UserId, banDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            banRepositoryMock.Verify(r => r.GetByIdAsync(ban.UserId), Times.Once);
            banRepositoryMock.Verify(r => r.UpdateAsync(ban, updatedBan), Times.Once);
        }

        [TestMethod]
        public void Update_BanDoesNotExist_ReturnsNotFound()
        {
            JwtService.AuthentifyController(controller, admin);
            banRepositoryMock.Setup(r => r.GetByIdAsync(99))
                              .ReturnsAsync((Ban?)null);

            // When
            IActionResult? result = controller.Update(99, banDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            banRepositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
            banRepositoryMock.Verify(r => r.UpdateAsync(ban, ban), Times.Never);
        }

        [TestMethod]
        public void Update_InvalidModelState_ReturnsBadRequest()
        {
            JwtService.AuthentifyController(controller, admin);
            controller.ModelState.AddModelError("BanReason", "Required");

            // When
            IActionResult? result = controller.Update(ban.UserId, banDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            banRepositoryMock.Verify(r => r.GetByIdAsync(ban.UserId), Times.Never);
            banRepositoryMock.Verify(r => r.UpdateAsync(ban, ban), Times.Never);
        }

        #endregion
    }
}