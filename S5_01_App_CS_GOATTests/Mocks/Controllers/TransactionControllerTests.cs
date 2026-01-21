using Microsoft.Extensions.Configuration;
using Moq;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOATTests.Fixtures;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class TransactionControllerTests
    {
        private Mock<IConfiguration> configurationMock;
        private TransactionController? controller;

        private User? admin;
        private User? normalUser;

        [TestInitialize]
        public void Initialize()
        {
            configurationMock = new Mock<IConfiguration>();

            admin = UserFixture.GetAdminUser();
            normalUser = UserFixture.GetNormalUser();

            controller = new TransactionController(
                configurationMock.Object
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
            _ = Assert.ThrowsException<NotImplementedException>(() =>
            {
                _ = controller.Delete(1).GetAwaiter().GetResult();
            });
        }

        #endregion
    }
}
