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
using System.Threading;
using System.Threading.Tasks;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class RandomTransactionControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<RandomTransaction, int>>? transactionRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private RandomTransactionController? controller;

        private User? admin;
        private User? normalUser;
        private List<RandomTransaction>? transactions;
        private List<RandomTransactionDTO>? transactionDTOs;
        private RandomTransaction? transaction;
        private RandomTransactionDetailDTO? transactionDetailDTO;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            transactionRepositoryMock = new Mock<IDataRepository<RandomTransaction, int>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            transactions = TransactionFixture.GetRandomTransactions();
            transactionDTOs = TransactionFixture.GetRandomTransactionDTOs();
            transaction = TransactionFixture.GetRandomTransaction();
            transactionDetailDTO = TransactionFixture.GetRandomTransactionDetailDTO();

            controller = new RandomTransactionController(
                mapperMock.Object,
                transactionRepositoryMock.Object,
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
            transactionRepositoryMock.Setup(r => r.GetAllAsync(null)).ReturnsAsync(transactions);
            mapperMock.Setup(m => m.Map<IEnumerable<RandomTransactionDTO>>(transactions))
                       .Returns(transactionDTOs);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        [TestMethod]
        public void GetAll_AsNonAdmin_ReturnsForbidden()
        {
            JwtService.AuthentifyController(controller, normalUser);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetAll_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetByUser_AuthenticatedUser_ReturnsOwnTransactions()
        {
            JwtService.AuthentifyController(controller, normalUser);
            transactionRepositoryMock.Setup(r => r.GetAllAsync(null)).ReturnsAsync(transactions);
            mapperMock.Setup(m => m.Map<IEnumerable<RandomTransactionDTO>>(transactions))
                       .Returns(transactionDTOs);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        [TestMethod]
        public void GetByUser_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void Get_ValidId_ReturnsOk()
        {
            JwtService.AuthentifyController(controller, normalUser);
            transactionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(transaction);
            mapperMock.Setup(m => m.Map<RandomTransactionDetailDTO>(transaction))
                       .Returns(transactionDetailDTO);

            // When
            IActionResult? result = controller.Get(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            transactionRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [TestMethod]
        public void Get_InvalidId_ReturnsNotFound()
        {
            JwtService.AuthentifyController(controller, normalUser);
            transactionRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((RandomTransaction?)null);

            // When
            IActionResult? result = controller.Get(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            transactionRepositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [TestMethod]
        public void Get_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.Get(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            transactionRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Never);
        }

        #endregion
    }
}
