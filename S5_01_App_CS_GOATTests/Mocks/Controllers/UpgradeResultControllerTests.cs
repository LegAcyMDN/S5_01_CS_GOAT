using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.Controllers;
using Shared.DTO;
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
        private Mock<IDataRepository<InventoryItem, int>>? inventoryItemRepositoryMock;
        private Mock<IDataRepository<RandomTransaction, int>>? randomTransactionRepositoryMock;
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
            inventoryItemRepositoryMock = new Mock<IDataRepository<InventoryItem, int>>();
            randomTransactionRepositoryMock = new Mock<IDataRepository<RandomTransaction, int>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            upgradeResults = TransactionFixture.GetUpgradeResults();
            upgradeResultDTOs = TransactionFixture.GetUpgradeResultDTOs();

            controller = new UpgradeResultController(
                mapperMock.Object,
                inventoryItemRepositoryMock.Object,
                randomTransactionRepositoryMock.Object,
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
            int inventoryItemId = 1;
            JwtService.AuthentifyController(controller, normalUser);
            var inventoryItem = new InventoryItem
            {
                InventoryItemId = inventoryItemId,
                UserId = normalUser.UserId,
                WearId = 1,
                Float = 0.15f,
                AcquiredOn = DateTime.Now,
                IsFavorite = false,
                UpgradeResults = upgradeResults.Where(ur => ur.InventoryItemId == inventoryItemId).ToList()
            };

            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()))
                                       .ReturnsAsync(inventoryItem);
            mapperMock.Setup(m => m.Map<IEnumerable<UpgradeResultDTO>>(inventoryItem.UpgradeResults))
                       .Returns(upgradeResultDTOs);

            // When
            IActionResult? result = controller.GetByInventoryItem(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetByInventoryItem_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByInventoryItem(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GetByRandomTransaction_AuthenticatedUser_ReturnsOk()
        {
            int transactionId = 1;
            JwtService.AuthentifyController(controller, normalUser);
            var randomTransaction = new RandomTransaction
            {
                TransactionId = transactionId,
                UserId = normalUser.UserId,
                WalletValue = 100,
                TransactionDate = DateTime.Now,
                UpgradeResults = upgradeResults.Where(ur => ur.TransactionId == transactionId).ToList()
            };

            randomTransactionRepositoryMock.Setup(r => r.GetByIdAsyncNew(transactionId, It.IsAny<QueryOptions<RandomTransaction>>()))
                                           .ReturnsAsync(randomTransaction);
            mapperMock.Setup(m => m.Map<IEnumerable<UpgradeResultDTO>>(randomTransaction.UpgradeResults))
                       .Returns(upgradeResultDTOs);

            // When
            IActionResult? result = controller.GetByRandomTransaction(transactionId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetByRandomTransaction_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByRandomTransaction(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GetByInventoryItem_InvalidId_ReturnsNotFound()
        {
            JwtService.AuthentifyController(controller, normalUser);
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(999, It.IsAny<QueryOptions<InventoryItem>>()))
                                       .ReturnsAsync((InventoryItem?)null);

            // When
            IActionResult? result = controller.GetByInventoryItem(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetByInventoryItem_NegativeId_ReturnsNotFound()
        {
            JwtService.AuthentifyController(controller, normalUser);
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(-1, It.IsAny<QueryOptions<InventoryItem>>()))
                                       .ReturnsAsync((InventoryItem?)null);

            // When
            IActionResult? result = controller.GetByInventoryItem(-1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetByRandomTransaction_InvalidId_ReturnsNotFound()
        {
            JwtService.AuthentifyController(controller, normalUser);
            randomTransactionRepositoryMock.Setup(r => r.GetByIdAsyncNew(999, It.IsAny<QueryOptions<RandomTransaction>>()))
                                           .ReturnsAsync((RandomTransaction?)null);

            // When
            IActionResult? result = controller.GetByRandomTransaction(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion
    }
}

