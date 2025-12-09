using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOATTests.Fixtures;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class PaymentMethodControllerTests
    {
        private Mock<ITypeRepository<PaymentMethod>>? paymentMethodRepositoryMock;
        private PaymentMethodController? controller;

        private List<PaymentMethod>? paymentMethods;

        [TestInitialize]
        public void Initialize()
        {
            paymentMethodRepositoryMock = new Mock<ITypeRepository<PaymentMethod>>();

            paymentMethods = TransactionFixture.GetPaymentMethods();

            controller = new PaymentMethodController(
                paymentMethodRepositoryMock.Object
            );
        }

        #region GetAll Tests

        [TestMethod]
        public void GetAll_ReturnsOk()
        {
            // Given
            paymentMethodRepositoryMock.Setup(r => r.GetAllAsync())
                                       .ReturnsAsync(paymentMethods);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            paymentMethodRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        #endregion
    }
}
