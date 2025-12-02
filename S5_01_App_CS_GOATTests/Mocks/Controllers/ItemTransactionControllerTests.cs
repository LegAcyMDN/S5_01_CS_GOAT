using System;
using System.Collections.Generic;
using System.Threading;
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

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class ItemTransactionControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<ItemTransaction, int>>? itemTransactionRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private ItemTransactionController? controller;

        private User? admin;
        private User? normalUser;
        private ItemTransaction? itemTransaction;
        private ItemTransaction? otherUserItemTransaction;
        private List<ItemTransaction>? itemTransactions;
        private ItemTransactionDTO? itemTransactionDTO;
        private ItemTransactionDetailDTO? itemTransactionDetailDTO;
        private List<ItemTransactionDTO>? itemTransactionDTOs;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            itemTransactionRepositoryMock = new Mock<IDataRepository<ItemTransaction, int>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            itemTransaction = ItemTransactionFixture.GetItemTransaction();
            otherUserItemTransaction = ItemTransactionFixture.GetOtherUserItemTransaction();
            itemTransactions = ItemTransactionFixture.GetItemTransactions();
            itemTransactionDTO = ItemTransactionFixture.GetItemTransactionDTO();
            itemTransactionDetailDTO = ItemTransactionFixture.GetItemTransactionDetailDTO();
            itemTransactionDTOs = ItemTransactionFixture.GetItemTransactionDTOs();

            controller = new ItemTransactionController(
                mapperMock.Object,
                itemTransactionRepositoryMock.Object,
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
            itemTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
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
            itemTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetAll_AsAdmin_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            itemTransactionRepositoryMock.Setup(r => r.GetAllAsync(null))
                                         .ReturnsAsync(itemTransactions);
            mapperMock.Setup(m => m.Map<IEnumerable<ItemTransactionDTO>>(itemTransactions))
                      .Returns(itemTransactionDTOs);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            itemTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion

        #region Get Tests

        [TestMethod]
        public void Get_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.Get(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            itemTransactionRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Never);
        }

        [TestMethod]
        public void Get_ExistingOwnTransaction_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int transactionId = 1;
            
            itemTransactionRepositoryMock.Setup(r => r.GetByIdAsync(transactionId))
                                         .ReturnsAsync(itemTransaction);
            mapperMock.Setup(m => m.Map<ItemTransactionDetailDTO>(itemTransaction))
                      .Returns(itemTransactionDetailDTO);

            // When
            IActionResult? result = controller.Get(transactionId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            itemTransactionRepositoryMock.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        }

        [TestMethod]
        public void Get_NonExistingTransaction_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int transactionId = 999;
            
            itemTransactionRepositoryMock.Setup(r => r.GetByIdAsync(transactionId))
                                         .ReturnsAsync((ItemTransaction?)null);

            // When
            IActionResult? result = controller.Get(transactionId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            itemTransactionRepositoryMock.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        }

        [TestMethod]
        public void Get_OtherUserTransaction_AsNonAdmin_ReturnsForbid()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int transactionId = 2;
            
            itemTransactionRepositoryMock.Setup(r => r.GetByIdAsync(transactionId))
                                         .ReturnsAsync(otherUserItemTransaction);

            // When
            IActionResult? result = controller.Get(transactionId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            itemTransactionRepositoryMock.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        }

        [TestMethod]
        public void Get_OtherUserTransaction_AsAdmin_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            int transactionId = 2;
            
            itemTransactionRepositoryMock.Setup(r => r.GetByIdAsync(transactionId))
                                         .ReturnsAsync(otherUserItemTransaction);
            mapperMock.Setup(m => m.Map<ItemTransactionDetailDTO>(otherUserItemTransaction))
                      .Returns(itemTransactionDetailDTO);

            // When
            IActionResult? result = controller.Get(transactionId).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            itemTransactionRepositoryMock.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        }

        #endregion

        #region GetByUser Tests

        [TestMethod]
        public void GetByUser_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            itemTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetByUser_Authenticated_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            itemTransactionRepositoryMock.Setup(r => r.GetAllAsync(null))
                                         .ReturnsAsync(itemTransactions);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            itemTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion
    }
}
