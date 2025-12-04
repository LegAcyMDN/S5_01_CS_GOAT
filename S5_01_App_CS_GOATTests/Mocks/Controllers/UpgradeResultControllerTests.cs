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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class UpgradeResultControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<UpgradeResult, int>>? upgradeResultRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private UpgradeResultController? controller;

        private User? admin;
        private User? normalUser;
        private List<UpgradeResult>? upgradeResults;
        private List<UpgradeResultDTO>? upgradeResultDTOs;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            upgradeResultRepositoryMock = new Mock<IDataRepository<UpgradeResult, int>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            upgradeResults = TransactionFixture.GetUpgradeResults();
            upgradeResultDTOs = TransactionFixture.GetUpgradeResultDTOs();

            controller = new UpgradeResultController(
                mapperMock.Object,
                upgradeResultRepositoryMock.Object,
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
        public void GetByInventoryItem_AuthenticatedUser_ReturnsOk()
        {
            JwtService.AuthentifyController(controller, normalUser);
            upgradeResultRepositoryMock.Setup(r => r.GetAllAsync(null))
                                       .ReturnsAsync(upgradeResults);
            mapperMock.Setup(m => m.Map<IEnumerable<UpgradeResultDTO>>(upgradeResults))
                       .Returns(upgradeResultDTOs);

            // When
            IActionResult? result = controller.GetByInventoryItem(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            upgradeResultRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        [TestMethod]
        public void GetByInventoryItem_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByInventoryItem(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            upgradeResultRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetByRandomTransaction_AuthenticatedUser_ReturnsOk()
        {
            JwtService.AuthentifyController(controller, normalUser);
            upgradeResultRepositoryMock.Setup(r => r.GetAllAsync(null))
                                       .ReturnsAsync(upgradeResults);
            mapperMock.Setup(m => m.Map<IEnumerable<UpgradeResultDTO>>(upgradeResults))
                       .Returns(upgradeResultDTOs);

            // When
            IActionResult? result = controller.GetByRandomTransaction(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            upgradeResultRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        [TestMethod]
        public void GetByRandomTransaction_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByRandomTransaction(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            upgradeResultRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        #endregion
    }
}
