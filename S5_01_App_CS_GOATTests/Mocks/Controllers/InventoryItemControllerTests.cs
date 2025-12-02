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
    public class InventoryItemControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<InventoryItem, int>>? inventoryItemRepositoryMock;
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
            configurationMock = new Mock<IConfiguration>();

            normalUser = UserFixture.GetNormalUser();
            inventoryItem = InventoryItemFixture.GetInventoryItem();
            otherUserInventoryItem = InventoryItemFixture.GetOtherUserInventoryItem();
            inventoryItems = InventoryItemFixture.GetInventoryItems();
            inventoryItemDTO = InventoryItemFixture.GetInventoryItemDTO();
            inventoryItemDetailDTO = InventoryItemFixture.GetInventoryItemDetailDTO();
            inventoryItemDTOs = InventoryItemFixture.GetInventoryItemDTOs();

            controller = new InventoryItemController(
                inventoryItemRepositoryMock.Object,
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
            inventoryItemRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetByUser_Authenticated_ReturnsOkWithInventory()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            inventoryItemRepositoryMock.Setup(r => r.GetAllAsync(null))
                                       .ReturnsAsync(inventoryItems);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            inventoryItemRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
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
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Never);
        }

        [TestMethod]
        public void GetDetails_ExistingOwnItem_ReturnsOkWithDetails()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 1;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsync(inventoryItemId))
                                       .ReturnsAsync(inventoryItem);
            mapperMock.Setup(m => m.Map<InventoryItemDetailDTO>(inventoryItem))
                      .Returns(inventoryItemDetailDTO);

            // When
            IActionResult? result = controller.GetDetails(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(inventoryItemId), Times.Once);
        }

        [TestMethod]
        public void GetDetails_NonExistingItem_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 999;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsync(inventoryItemId))
                                       .ReturnsAsync((InventoryItem?)null);

            // When
            IActionResult? result = controller.GetDetails(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(inventoryItemId), Times.Once);
        }

        [TestMethod]
        public void GetDetails_ItemBelongsToOtherUser_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 2;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsync(inventoryItemId))
                                       .ReturnsAsync(otherUserInventoryItem);

            // When
            IActionResult? result = controller.GetDetails(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(inventoryItemId), Times.Once);
        }

        #endregion

        #region Upgrade Tests

        [TestMethod]
        public void Upgrade_CallsMethod_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);

            // When
            IActionResult? result = controller.Upgrade().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void Upgrade_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.Upgrade().GetAwaiter().GetResult();

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
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Never);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Never);
        }

        [TestMethod]
        public void ToggleFavorite_ExistingOwnItem_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 1;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsync(inventoryItemId))
                                       .ReturnsAsync(inventoryItem);
            inventoryItemRepositoryMock.Setup(r => r.UpdateAsync(inventoryItem, inventoryItem))
                                       .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.ToggleFavorite(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(inventoryItemId), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Once);
        }

        [TestMethod]
        public void ToggleFavorite_NonExistingItem_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 999;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsync(inventoryItemId))
                                       .ReturnsAsync((InventoryItem?)null);

            // When
            IActionResult? result = controller.ToggleFavorite(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(inventoryItemId), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Never);
        }

        [TestMethod]
        public void ToggleFavorite_ItemBelongsToOtherUser_ReturnsForbid()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 2;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsync(inventoryItemId))
                                       .ReturnsAsync(otherUserInventoryItem);

            // When
            IActionResult? result = controller.ToggleFavorite(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(inventoryItemId), Times.Once);
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
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Never);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Never);
        }

        [TestMethod]
        public void Sell_ExistingOwnItem_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 1;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsync(inventoryItemId))
                                       .ReturnsAsync(inventoryItem);
            inventoryItemRepositoryMock.Setup(r => r.UpdateAsync(inventoryItem, inventoryItem))
                                       .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.Sell(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(inventoryItemId), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Once);
        }

        [TestMethod]
        public void Sell_NonExistingItem_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 999;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsync(inventoryItemId))
                                       .ReturnsAsync((InventoryItem?)null);

            // When
            IActionResult? result = controller.Sell(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(inventoryItemId), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(inventoryItem, inventoryItem), Times.Never);
        }

        [TestMethod]
        public void Sell_ItemBelongsToOtherUser_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int inventoryItemId = 2;
            
            inventoryItemRepositoryMock.Setup(r => r.GetByIdAsync(inventoryItemId))
                                       .ReturnsAsync(otherUserInventoryItem);

            // When
            IActionResult? result = controller.Sell(inventoryItemId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            inventoryItemRepositoryMock.Verify(r => r.GetByIdAsync(inventoryItemId), Times.Once);
            inventoryItemRepositoryMock.Verify(r => r.UpdateAsync(otherUserInventoryItem, otherUserInventoryItem), Times.Never);
        }

        #endregion
    }
}
