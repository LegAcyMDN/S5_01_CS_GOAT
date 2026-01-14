using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Mapper;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class InventoryItemControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<InventoryItem, int>>? inventoryItemRepositoryMock;
        private Mock<ISellingRepository>? sellingServiceMock;
        private Mock<IConfiguration>? configurationMock;
        private InventoryItemController? controller;

        private User? normalUser;
        private InventoryItem? inventoryItem;
        private InventoryItem? otherUserInventoryItem;
        private List<InventoryItem>? inventoryItems;
        private InventoryItemDTO? inventoryItemDTO;
        private InventoryItemDetailDTO? inventoryItemDetailDTO;
        private List<InventoryItemDTO>? inventoryItemDTOs;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            inventoryItemRepositoryMock = new Mock<IDataRepository<InventoryItem, int>>();
            sellingServiceMock = new Mock<ISellingRepository>();
            configurationMock = new Mock<IConfiguration>();

            normalUser = UserFixture.GetNormalUser();
            inventoryItem = InventoryItemFixture.GetInventoryItem();
            otherUserInventoryItem = InventoryItemFixture.GetOtherUserInventoryItem();
            inventoryItems = InventoryItemFixture.GetInventoryItems();
            inventoryItemDTO = InventoryItemFixture.GetInventoryItemDTO();
            inventoryItemDetailDTO = InventoryItemFixture.GetInventoryItemDetailDTO();
            inventoryItemDTOs = InventoryItemFixture.GetInventoryItemDTOs();

            Mock<IUpgradeRepository> upgradeServiceMock = new Mock<IUpgradeRepository>();
            controller = new InventoryItemController(
                inventoryItemRepositoryMock.Object,
                upgradeServiceMock.Object,
                sellingServiceMock.Object,
                mapperMock.Object,
                configurationMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region GetByUser Tests

        [TestMethod]
        public void GetByUser_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            inventoryItemRepositoryMock.Verify(r => r.GetAllAsyncNew(null), Times.Never);
        }

        [TestMethod]
        public void GetByUser_Authenticated_ReturnsOkWithInventory()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            inventoryItemRepositoryMock.Setup(r => r.GetAllAsyncOld(
                i => i.RemovedOn == null, "Wear.Skin.Rarity"
                )).ReturnsAsync(inventoryItems);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            inventoryItemRepositoryMock.Verify(r => r.GetAllAsyncOld(
                i => i.RemovedOn == null, "Wear.Skin.Rarity"
                ), Times.Once);
        }

        #endregion

        #region GetDetails Tests

        [TestMethod]
        public void GetDetails_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetDetails(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(1, It.IsAny<QueryOptions<InventoryItem>>()), Times.Never);
        }

        [TestMethod]
        public void GetDetails_ExistingOwnItem_ReturnsOkWithDetails()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 1;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(
                inventoryItemId,
                It.IsAny<QueryOptions<InventoryItem>>()
            ))
                                       .ReturnsAsync(inventoryItem);
            mapperMock.Setup(m => m.Map<InventoryItemDetailDTO>(inventoryItem))
                      .Returns(inventoryItemDetailDTO);

            // When
            IActionResult? result = controller.GetDetails(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(inventoryItemId,
                It.IsAny<QueryOptions<InventoryItem>>()
            ), Times.Once);
        }

        [TestMethod]
        public void GetDetails_NonExistingItem_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 999;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(inventoryItemId,
                It.IsAny<QueryOptions<InventoryItem>>()
            )).ReturnsAsync((InventoryItem?)null);

            // When
            IActionResult? result = controller.GetDetails(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(inventoryItemId,
                It.IsAny<QueryOptions<InventoryItem>>()
            ), Times.Once);
        }

        [TestMethod]
        public void GetDetails_ItemBelongsToOtherUser_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 2;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(inventoryItemId,
                It.IsAny<QueryOptions<InventoryItem>>()
            )).ReturnsAsync(otherUserInventoryItem);

            // When
            IActionResult? result = controller.GetDetails(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(inventoryItemId,
                It.IsAny<QueryOptions<InventoryItem>>()
            ), Times.Once);
        }

        #endregion

        #region Upgrade Tests

        [TestMethod]
        public void Upgrade_Authentificated_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            var upgradeDto = new Shared.DTO.Helpers.UpgradeInputDTO
            {
                InventoryItemIds = new List<int> { 1, 2 },
                TargetSkinId = 1,
                MonetaryValue = 10.0
            };

            // When
            IActionResult? result = controller.Upgrade(upgradeDto).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void Upgrade_Unauthenticated_ReturnsUnauthorized()
        {
            var upgradeDto = new Shared.DTO.Helpers.UpgradeInputDTO
            {
                InventoryItemIds = new List<int> { 1, 2 },
                TargetSkinId = 1,
                MonetaryValue = 10.0
            };

            // When
            IActionResult? result = controller.Upgrade(upgradeDto).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        #endregion

        #region ToggleFavorite Tests

        [TestMethod]
        public void ToggleFavorite_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.ToggleFavorite(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(1, It.IsAny<QueryOptions<InventoryItem>>()), Times.Never);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Never);
        }

        [TestMethod]
        public void ToggleFavorite_ExistingOwnItem_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 1;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()))
                                       .ReturnsAsync(inventoryItem);
            inventoryItemRepositoryMock.Setup(r => r.UpdateAsync(inventoryItem, inventoryItem))
                                       .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.ToggleFavorite(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Once);
        }

        [TestMethod]
        public void ToggleFavorite_NonExistingItem_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 999;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()))
                                       .ReturnsAsync((InventoryItem?)null);

            // When
            IActionResult? result = controller.ToggleFavorite(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Never);
        }

        [TestMethod]
        public void ToggleFavorite_ItemBelongsToOtherUser_ReturnsForbid()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 2;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()))
                                       .ReturnsAsync(otherUserInventoryItem);

            // When
            IActionResult? result = controller.ToggleFavorite(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(otherUserInventoryItem, otherUserInventoryItem), Times.Never);
        }

        #endregion

        #region Sell Tests

        [TestMethod]
        public void Sell_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.Sell(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(1, It.IsAny<QueryOptions<InventoryItem>>()), Times.Never);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Never);
        }

        [TestMethod]
        public void Sell_ExistingOwnItem_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 1;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()))
                                       .ReturnsAsync(inventoryItem);
            sellingServiceMock.Setup(r => r.SellAsync(inventoryItemId))
                                        .ReturnsAsync(StatusCodes.Status204NoContent);

            // When
            IActionResult? result = controller.Sell(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(((StatusCodeResult)result).StatusCode, 204);
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()), Times.Once);
            sellingServiceMock.Verify(r => r.SellAsync(inventoryItemId), Times.Once);
        }

        [TestMethod]
        public void Sell_NonExistingItem_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 999;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()))
                                       .ReturnsAsync((InventoryItem?)null);

            // When
            IActionResult? result = controller.Sell(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Never);
        }

        [TestMethod]
        public void Sell_ItemBelongsToOtherUser_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 2;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()))
                                       .ReturnsAsync(otherUserInventoryItem);

            // When
            IActionResult? result = controller.Sell(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsyncNew(inventoryItemId, It.IsAny<QueryOptions<InventoryItem>>()), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(otherUserInventoryItem, otherUserInventoryItem), Times.Never);
        }

        #endregion
    }
}


