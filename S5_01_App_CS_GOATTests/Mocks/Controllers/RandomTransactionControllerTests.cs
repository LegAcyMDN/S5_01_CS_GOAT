using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;
using Shared.DTO;

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
            _ = transactionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<RandomTransaction>?>())).ReturnsAsync(transactions);
            _ = mapperMock.Setup(m => m.Map<IEnumerable<RandomTransactionDTO>>(transactions))
                       .Returns(transactionDTOs);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<RandomTransaction>?>()), Times.Once);
        }

        [TestMethod]
        public void GetAll_AsNonAdmin_ReturnsForbidden()
        {
            JwtService.AuthentifyController(controller, normalUser);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<RandomTransaction>?>()), Times.Never);
        }

        [TestMethod]
        public void GetAll_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<RandomTransaction>?>()), Times.Never);
        }

        [TestMethod]
        public void GetByUser_AuthenticatedUser_ReturnsOwnTransactions()
        {
            JwtService.AuthentifyController(controller, normalUser);
            _ = transactionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<RandomTransaction>?>())).ReturnsAsync(transactions);
            _ = mapperMock.Setup(m => m.Map<IEnumerable<RandomTransactionDTO>>(transactions))
                       .Returns(transactionDTOs);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<RandomTransaction>?>()), Times.Once);
        }

        [TestMethod]
        public void GetByUser_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            transactionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<RandomTransaction>?>()), Times.Never);
        }

        [TestMethod]
        public void Get_ValidId_ReturnsOk()
        {
            JwtService.AuthentifyController(controller, normalUser);
            _ = transactionRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<RandomTransaction>?>())
            ).ReturnsAsync(transaction);
            _ = mapperMock.Setup(m => m.Map<RandomTransactionDetailDTO>(transaction))
                       .Returns(transactionDetailDTO);

            // When
            IActionResult? result = controller.Get(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            transactionRepositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<RandomTransaction>?>()), Times.Once);
        }

        [TestMethod]
        public void Get_InvalidId_ReturnsNotFound()
        {
            JwtService.AuthentifyController(controller, normalUser);
            _ = transactionRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<QueryOptions<RandomTransaction>?>())
            ).ReturnsAsync((RandomTransaction?)null);

            // When
            IActionResult? result = controller.Get(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            transactionRepositoryMock.Verify(r => r.GetByIdAsync(999, It.IsAny<QueryOptions<RandomTransaction>?>()), Times.Once);
        }

        [TestMethod]
        public void Get_Unauthenticated_ReturnsUnauthorized()
        {
            // When
            IActionResult? result = controller.Get(1).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            transactionRepositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<RandomTransaction>>()), Times.Never);
        }

        #endregion

        #region LiveFeed Tests

        [TestMethod]
        public void LiveFeed_ReturnsDTOs()
        {
            // Given
            _ = transactionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<RandomTransaction>>())).ReturnsAsync(transactions);
            _ = mapperMock.Setup(m => m.Map<IEnumerable<LiveFeedDTO>>(transactions))
                       .Returns(new List<LiveFeedDTO>());

            // When
            IActionResult? result = controller.LiveFeed().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        #endregion
    }
}




