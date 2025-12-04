using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
    public class LimitControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<Limit, (int, int)>>? limitRepositoryMock;
        private Mock<ITypeRepository<LimitType>>? limitTypeRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private LimitController? controller;

        private User? normalUser;
        private Limit? limit;
        private List<Limit>? limits;
        private LimitType? limitType;
        private List<LimitType>? limitTypes;
        private LimitDTO? limitDTO;
        private LimitDTO? updatedLimitDTO;
        private List<LimitDTO>? limitDTOs;
        
        private (int, int) limitKey;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            limitRepositoryMock = new Mock<IDataRepository<Limit, (int, int)>>();
            limitTypeRepositoryMock = new Mock<ITypeRepository<LimitType>>();
            configurationMock = new Mock<IConfiguration>();

            normalUser = UserFixture.GetNormalUser();
            limit = LimitFixture.GetLimit();
            limits = LimitFixture.GetLimits();
            limitType = LimitTypeFixture.GetLimitType();
            limitTypes = LimitTypeFixture.GetLimitTypes();
            limitDTO = LimitFixture.GetLimitDTO();
            updatedLimitDTO = LimitFixture.GetUpdatedLimitDTO();
            limitDTOs = LimitFixture.GetLimitDTOs();

            limitKey = LimitFixture.GetLimitKeyForNormalUser();

            controller = new LimitController(
                mapperMock.Object,
                limitRepositoryMock.Object,
                limitTypeRepositoryMock.Object,
                configurationMock.Object
            );

            limitTypeRepositoryMock.Setup(r => r.GetTypeByNameAsync(limitType.LimitTypeName))
                               .ReturnsAsync(limitType);
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
            limitRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetByUser_Authenticated_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            limitRepositoryMock.Setup(r => r.GetAllAsync(null))
                               .ReturnsAsync(limits);
            mapperMock.Setup(m => m.Map<IEnumerable<LimitDTO>>(limits))
                      .Returns(limitDTOs);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            limitRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion

        #region Update Tests

        [TestMethod]
        public void Update_Unauthenticated_ReturnsUnauthorized()
        {
            // Given
            int limitTypeId = 1;

            // When
            IActionResult? result = controller.Update(updatedLimitDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            limitRepositoryMock.Verify(r => r.GetByIdAsync(limitKey), Times.Never);
        }

        [TestMethod]
        public void Update_ValidLimit_ReturnsNoContent()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int limitTypeId = 1;

            limitRepositoryMock.Setup(r => r.GetByIdAsync(limitKey))
                               .ReturnsAsync(limit);
            limitRepositoryMock.Setup(r => r.PatchAsync(limit, It.IsAny<Dictionary<string, object>>()))
                               .Returns(Task.CompletedTask);

            // When
            IActionResult? result = controller.Update(updatedLimitDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            limitRepositoryMock.Verify(r => r.GetByIdAsync(limitKey), Times.Once);
        }

        [TestMethod]
        public void Update_NonExistingLimit_ReturnsNotFound()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            int limitTypeId = 999;
            (int, int) nonExistingLimitKey = LimitFixture.GetLimitKey(normalUser.UserId, limitTypeId);

            limitRepositoryMock.Setup(r => r.GetByIdAsync(nonExistingLimitKey))
                               .ReturnsAsync((Limit?)null);

            // When
            IActionResult? result = controller.Update(updatedLimitDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            limitRepositoryMock.Verify(r => r.GetByIdAsync(nonExistingLimitKey), Times.Never);
        }

        [TestMethod]
        public void Update_InvalidModelState_ReturnsBadRequest()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            controller.ModelState.AddModelError("LimitAmount", "Required");
            int limitTypeId = 1;

            // When
            IActionResult? result = controller.Update(updatedLimitDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            limitRepositoryMock.Verify(r => r.GetByIdAsync(limitKey), Times.Never);
        }

        #endregion
    }
}
