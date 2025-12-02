using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class MoneyTransactionControllerTests
    {
        private Mock<IDataRepository<MoneyTransaction, int>>? moneyTransactionRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private MoneyTransactionController? controller;

        private User? admin;
        private User? normalUser;
        private MoneyTransaction? moneyTransaction;
        private List<MoneyTransaction>? moneyTransactions;
        private List<MoneyTransaction>? allMoneyTransactions;

        [TestInitialize]
        public void Initialize()
        {
            moneyTransactionRepositoryMock = new Mock<IDataRepository<MoneyTransaction, int>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();
            moneyTransaction = MoneyTransactionFixture.GetMoneyTransaction();
            moneyTransactions = MoneyTransactionFixture.GetMoneyTransactions();
            allMoneyTransactions = MoneyTransactionFixture.GetAllMoneyTransactions();

            controller = new MoneyTransactionController(
                moneyTransactionRepositoryMock.Object,
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
            moneyTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetByUser_Authenticated_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            moneyTransactionRepositoryMock.Setup(r => r.GetAllAsync(null))
                                          .ReturnsAsync(moneyTransactions);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            moneyTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
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
            moneyTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
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
            moneyTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetAll_AsAdmin_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, admin);
            moneyTransactionRepositoryMock.Setup(r => r.GetAllAsync(null))
                                          .ReturnsAsync(allMoneyTransactions);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            moneyTransactionRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion
    }
}
