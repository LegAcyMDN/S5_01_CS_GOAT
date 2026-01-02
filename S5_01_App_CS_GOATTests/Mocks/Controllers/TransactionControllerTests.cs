using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class TransactionControllerTests
    {
        private Mock<IDataRepository<Transaction, int>>? transactionRepositoryMock;
        private Mock<IConfiguration> configurationMock;
        private TransactionController? controller;

        private User? admin;
        private User? normalUser;

        [TestInitialize]
        public void Initialize()
        {
            transactionRepositoryMock = new Mock<IDataRepository<Transaction, int>>();
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();

            controller = new TransactionController(
                configurationMock.Object,
                transactionRepositoryMock.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = null;
        }

        #region DELETE Tests

        [TestMethod]
        public void Delete_NotImplemented_ThrowsNotImplementedException()
        {
            JwtService.AuthentifyController(controller, admin);

            // When/Then
            Assert.ThrowsException<NotImplementedException>(() =>
            {
                controller.Delete(1).GetAwaiter().GetResult();
            });
        }

        #endregion
    }
}
