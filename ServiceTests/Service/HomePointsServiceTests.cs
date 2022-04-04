using AutoMapper;
using Domain.DTOs;
using ExternalServices.Facades;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using FluentAssertions;
using Refit;
using Service.Service;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using AutoFixture;

namespace Tests.Service
{
    public class HomePointsServiceTests
    {
        private readonly Mock<IHomeFamilyPointsFacade> _mockFacade;
        private readonly Mock<ILogger<HomePointsService>> _mockLogService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Fixture _fixture;

        private HomePointsService _service;

        public HomePointsServiceTests()
        {
            _mockFacade = new Mock<IHomeFamilyPointsFacade>();
            _mockLogService = new Mock<ILogger<HomePointsService>>();
            _mockMapper = new Mock<IMapper>();

            #region .: Setup :.

            #region .: Mocks :.
            var dep1 = new DependentExternal() { Age = 10, Relationship = "Filho" };
            var dep2 = new DependentExternal() { Age = 15, Relationship = "Filha" };
            var dep3 = new DependentExternal() { Age = 40, Relationship = "Esposa" };
            var dep4 = new DependentExternal() { Age = 12, Relationship = "Sobrinha" };

            var lstFamilyMember1 = new List<FamilyMembersExternal>() {
                new FamilyMembersExternal() { Dependent = dep1, IsDependent = true },
                new FamilyMembersExternal(){ Dependent = dep3, IsDependent = false},
            };

            var lstFamilyMember2 = new List<FamilyMembersExternal>() {
                new FamilyMembersExternal() { Dependent = dep2, IsDependent = true},
                new FamilyMembersExternal(){ Dependent = dep4, IsDependent = false},
            };

            var lstFamilyInformation = new List<FamilyInformationExternal>() { 
                new FamilyInformationExternal() { Id = 1, Income = 1000, FamilyMembers = lstFamilyMember1 },
                new FamilyInformationExternal() { Id = 2, Income = 1600, FamilyMembers = lstFamilyMember2 }
            };
            #endregion

            #endregion

            _service = new HomePointsService(_mockFacade.Object, _mockLogService.Object, _mockMapper.Object);
        }

        private ApiResponse<T> CreateResponse<T>(T content, HttpStatusCode statusCode)
        {
            var httpResponseMessage = new HttpResponseMessage(statusCode);
            httpResponseMessage.RequestMessage = new HttpRequestMessage();
            httpResponseMessage.RequestMessage.RequestUri = new Uri("http://teste.com");
            httpResponseMessage.RequestMessage.Method = new HttpMethod("GET");
            return new ApiResponse<T>(httpResponseMessage, content, new());
        }

        private ISetup<IHomeFamilyPointsFacade, Task<ApiResponse<FamilyInformationExternal>>> SetupGetFamilyById() =>
            _mockFacade.Setup(f => f.GetFamilyById(It.IsAny<IDictionary<string, string>>(), It.IsAny<int>()));

        private ISetup<IHomeFamilyPointsFacade, Task<ApiResponse<IEnumerable<FamilyInformationExternal>>>> SetupGetFamilyInformation() =>
           _mockFacade.Setup(f => f.GetFamilyInformation(It.IsAny<IDictionary<string, string>>()));

        private ISetup<IHomeFamilyPointsFacade, Task<ApiResponse<int>>> SetupPostHomeFamilyPoints() =>
           _mockFacade.Setup(f => f.PostHomeFamilyPoints(It.IsAny<IDictionary<string, string>>(), It.IsAny<FamilyInformationExternal>()));


        [Fact]
        [Trait("GetFamilyPoints", "Deve retornar os pontos para uma familia que tem 2 dependentes, sendo um valido e outro não, com renda maior que 900 reais .")]
        public void ShouldReturnSuccessWhenFamilyHasAValidDependentAndIncomeGreaterThan900()
        {
            //Arrange
            #region .: Mocks :.
            var dep1 = new DependentExternal() { Age = 10, Relationship = "Filho" };
            var dep3 = new DependentExternal() { Age = 40, Relationship = "Esposa" };

            var lstFamilyMember1 = new List<FamilyMembersExternal>() {
                new FamilyMembersExternal() { Dependent = dep1, IsDependent = true },
                new FamilyMembersExternal(){ Dependent = dep3, IsDependent = false},
            };

            var lstFamilyInformation = new List<FamilyInformationExternal>() {
                new FamilyInformationExternal() { Id = 1, Income = 1000, FamilyMembers = lstFamilyMember1 },
            };

            Task<ApiResponse<IEnumerable<FamilyInformationExternal>>> mockApiResponse = (Task<ApiResponse<IEnumerable<FamilyInformationExternal>>>)lstFamilyInformation.AsEnumerable();
            var responseContent = _fixture.Build<IEnumerable<FamilyInformationExternal>>().Create();
            var response = CreateResponse(responseContent, HttpStatusCode.OK);

            SetupGetFamilyInformation().Returns(mockApiResponse);

            #endregion
           
            //Act
            var response1 = _service.GetFamilyPoints();


            //Assert
            response1.Should().NotBeNull();
        }

        [Fact]
        [Trait("GetFamilyPoints", "Deve retornar os pontos para uma familia que tem 2 dependentes, sendo um valido e outro não, com renda menor que 900 reais .")]
        public void ShouldReturnSuccessWhenFamilyHasAValidDependentAndIncomeLessThan900()
        {
            //Arrange
            #region .: Mocks :.
            var dep1 = new DependentExternal() { Age = 10, Relationship = "Filho" };
            var dep3 = new DependentExternal() { Age = 40, Relationship = "Esposa" };

            var lstFamilyMember1 = new List<FamilyMembersExternal>() {
                new FamilyMembersExternal() { Dependent = dep1, IsDependent = true },
                new FamilyMembersExternal(){ Dependent = dep3, IsDependent = false},
            };

            var lstFamilyInformation = new List<FamilyInformationExternal>() {
                new FamilyInformationExternal() { Id = 1, Income = 800, FamilyMembers = lstFamilyMember1 },
            };

            #endregion
            //Act

            var result = _service.GetFamilyPoints();
            //Assert
            result.Should().NotBeNull();
        }

        [Fact]
        [Trait("PaymentService", "200 - Deve retornar um status code de sucesso quando enviar uma request válida")]
        public async Task ShouldReturnSuccessResponse()
        {
            //Act
            
            var responseContent = _fixture.Build<IEnumerable<FamilyInformationExternal>>().Create();
            var response = CreateResponse(responseContent, HttpStatusCode.OK);

            SetupPaymentMethodAsync()
                .Returns(Task.FromResult(response));

            //Arrange
            var addPaymentMethodResponse = await _paymentService.AddPaymentMethod(request);

            //Assert
            Assert.NotNull(addPaymentMethodResponse);
            VerifyPaymentMethodAsyncRanOneTime();
        }
    }
}
