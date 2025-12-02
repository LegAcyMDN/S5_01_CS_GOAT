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
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace S5_01_App_CS_GOATTests.Mocks.Controllers
{
    [TestClass()]
    public class FairRandomControllerTests
    {
        private Mock<IMapper>? mapperMock;
        private Mock<IDataRepository<FairRandom, int>>? fairRandomRepositoryMock;
        private Mock<IConfiguration>? configurationMock;
        private FairRandomController? controller;

        private User? normalUser;
        private User? otherUser;
        private FairRandom? fairRandom;
        private FairRandomDTO? fairRandomDTO;
        private List<FairRandom>? fairRandomList;
        private List<FairRandomDTO>? fairRandomDTOList;

        [TestInitialize]
        public void Initialize()
        {
            mapperMock = new Mock<IMapper>();
            fairRandomRepositoryMock = new Mock<IDataRepository<FairRandom, int>>();
            configurationMock = new Mock<IConfiguration>();

            normalUser = TestFixture.GetNormalUser();
            otherUser = TestFixture.GetAdminUser();
            fairRandom = TestFixture.GetFairRandom();
            fairRandomDTO = TestFixture.GetSingleFairRandomDTO();
            fairRandomList = TestFixture.GetFairRandoms();
            fairRandomDTOList = TestFixture.GetFairRandomDTOs();

            controller = new FairRandomController(
                mapperMock.Object,
                fairRandomRepositoryMock.Object,
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
            fairRandomRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Never);
        }

        [TestMethod]
        public void GetByUser_AuthenticatedUser_ReturnsFairRandoms()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            
            List<FairRandom> mixedFairRandomList = TestFixture.GetFairRandoms();
            fairRandomRepositoryMock.Setup(r => r.GetAllAsync(null))
                                    .ReturnsAsync(mixedFairRandomList);
            
            mapperMock.Setup(m => m.Map<IEnumerable<FairRandomDTO>>(mixedFairRandomList))
                      .Returns(fairRandomDTOList);

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            fairRandomRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        [TestMethod]
        public void GetByUser_AuthenticatedUser_EmptyList_ReturnsOk()
        {
            // Given
            JwtService.AuthentifyController(controller, normalUser);
            List<FairRandom> emptyList = new List<FairRandom>();
            fairRandomRepositoryMock.Setup(r => r.GetAllAsync(null))
                                    .ReturnsAsync(emptyList);
            
            mapperMock.Setup(m => m.Map<IEnumerable<FairRandomDTO>>(emptyList))
                      .Returns(new List<FairRandomDTO>());

            // When
            IActionResult? result = controller.GetByUser().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            fairRandomRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
        }

        #endregion
    }
}
