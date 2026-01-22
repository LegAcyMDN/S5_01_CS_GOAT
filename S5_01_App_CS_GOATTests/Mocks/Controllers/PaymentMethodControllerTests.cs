using Microsoft.AspNetCore.Mvc;
using Moq;
using S5_01_App_CS_GOAT.Controllers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
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
            _ = paymentMethodRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PaymentMethod>?>()))
                                       .ReturnsAsync(paymentMethods);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            paymentMethodRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<PaymentMethod>?>()), Times.Once);
        }

        [TestMethod]
        public void GetAll_EmptyList_ReturnsOk()
        {
            // Given
            var emptyList = new List<PaymentMethod>();
            _ = paymentMethodRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PaymentMethod>?>()))
                                       .ReturnsAsync(emptyList);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetAll_MultiplePaymentMethods_ReturnsOkWithAll()
        {
            // Given
            var multiplePaymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { PaymentMethodId = 1, PaymentMethodName = "Credit Card" },
                new PaymentMethod { PaymentMethodId = 2, PaymentMethodName = "PayPal" },
                new PaymentMethod { PaymentMethodId = 3, PaymentMethodName = "Bank Transfer" }
            };
            _ = paymentMethodRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PaymentMethod>?>()))
                                       .ReturnsAsync(multiplePaymentMethods);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            paymentMethodRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<PaymentMethod>?>()), Times.Once);
        }

        [TestMethod]
        public void GetAll_RepositoryCallsWithoutFilter_VerifiesCall()
        {
            // Given
            _ = paymentMethodRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<PaymentMethod>?>()))
                                       .ReturnsAsync(paymentMethods);

            // When
            IActionResult? result = controller.GetAll().GetAwaiter().GetResult();

            // Then
            paymentMethodRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<PaymentMethod>?>()), Times.Once());
            paymentMethodRepositoryMock.VerifyNoOtherCalls();
        }

        #endregion
    }
}

