using EntitiesRelations.API.Controllers;
using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;
using EntitiesRelations.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;

namespace EntitiesRelationsUnitTests
{
    public class CompanyControllerUnitTests
    {
        private Mock<ICompanyService> _mockCompanyService;
        private Mock<IOwnershipService> _mockOwnershipService;
        private readonly CompanyController _controller;

        public CompanyControllerUnitTests()
        {
            _mockCompanyService = new Mock<ICompanyService>();
            _mockOwnershipService = new Mock<IOwnershipService>();
            _controller = new CompanyController(_mockCompanyService.Object, _mockOwnershipService.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenCompanyServiceIsNull()
        {
            //Arrange + Act + Assert
            Assert.Throws<ArgumentNullException>(() => new CompanyController(null, _mockOwnershipService.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenOwnershipServiceIsNull()
        {
            //Arrange + Act + Assert
            Assert.Throws<ArgumentNullException>(() => new CompanyController(_mockCompanyService.Object, null));
        }

        [Fact]
        public void Constructor_DoesNotThrow_WhenArgumentsAreNotNull()
        {
            // Arrange
            ICompanyService companyService = new Mock<ICompanyService>().Object;
            IOwnershipService ownershipService = new Mock<IOwnershipService>().Object;

            // Act & Assert
            var controller = new CompanyController(companyService, ownershipService);
            Assert.NotNull(controller);
        }

        [Fact]
        public async Task GetAsync_ReturnsOkResult_WithListOfCompanies()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel (1, "ACME"),
                new CompanyModel (2, "Company2")
            };

            _mockCompanyService.Setup(service => service.GetAllCompanies()).ReturnsAsync(companies);

            // Act
            var result = await _controller.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CompanyResponseDTO>>(okResult.Value);
            Assert.Equal(companies.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetAsync_ReturnsOkResult_WithCompany()
        {
            // Arrange
            var companyId = 1;
            var company = new CompanyModel(1, "ACME");
            _mockCompanyService.Setup(service => service.GetByIdAsync(companyId)).ReturnsAsync(company);

            // Act
            var result = await _controller.GetAsync(companyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CompanyResponseDTO>(okResult.Value);
            Assert.Equal(companyId, returnValue.Id);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            var companyId = 1;
            _mockCompanyService.Setup(service => service.GetByIdAsync(companyId)).ReturnsAsync((CompanyModel)null);

            // Act
            var result = await _controller.GetAsync(companyId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Put_CompanyNotFound_ReturnsNotFound()
        {
            // Arrange
            var companyId = 1;
            var companyRequest = new CompanyRequestDTO { Id = companyId };

            _mockCompanyService.Setup(s => s.GetByIdAsync(companyId)).Returns(Task.FromResult<CompanyModel>(null));

            // Act
            var result = await _controller.Put(companyRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Could not find company with id {companyId}", notFoundResult.Value);
        }

        [Fact]
        public async Task Put_CompanyFound_ReturnsOkResult()
        {
            // Arrange
            var companyId = 1;
            var companyRequest = new CompanyRequestDTO { Id = companyId, Name = "Test Company" };
            var oldCompany = new CompanyModel(companyRequest.Id, companyRequest.Name);
            var updatedCompany = new CompanyModel (companyId,"Updated Company");

            var task = Task.FromResult(oldCompany);
            var updateTask = Task.FromResult(updatedCompany);

            _mockCompanyService.Setup(s => s.GetByIdAsync(companyRequest.Id)).Returns(task);
            _mockCompanyService.Setup(s => s.UpdateCompany(oldCompany)).Returns(updateTask);

            // Act
            var result = await _controller.Put(companyRequest);

            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CompanyResponseDTO>(okResult.Value);
            Assert.Equal(updatedCompany.Id, returnValue.Id);
            Assert.Equal(updatedCompany.Name, returnValue.Name);
            Assert.Equal(updatedCompany.AvailableShares, returnValue.AvailableShares);
            Assert.Equal(updatedCompany.WhoOwnsMeList, returnValue.WhoOwnsMe);
        }

        [Fact]
        public async Task Delete_CompanyNotFound_ReturnsNotFound()
        {
            // Arrange
            var companyId = 1;

            _mockCompanyService.Setup(s => s.GetByIdAsync(companyId)).Returns(Task.FromResult<CompanyModel>(null));

            // Act
            var result = await _controller.Delete(companyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Could not find company with id {companyId}", notFoundResult.Value);
        }

        [Fact]
        public async Task Delete_CompanyFound_ReturnsOkResult()
        {
            // Arrange
            var companyId = 1;
            var company = new CompanyModel(companyId, "Test Company");

            _mockCompanyService.Setup(s => s.GetByIdAsync(companyId)).Returns(Task.FromResult(company));
            _mockCompanyService.Setup(s => s.DeleteCompany(companyId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(companyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Company {companyId} deleted", okResult.Value);
        }

        [Fact]
        public async Task BuyCompany_ReturnsNotFound_WhenBuyerCompanyDoesNotExist()
        {
            // Arrange
            var buyerCompanyId = 1234;
            var companyId = 1235;
            var percentage = 50;
            _mockCompanyService.Setup(service => service.GetByIdAsync(buyerCompanyId)).ReturnsAsync((CompanyModel)null);

            // Act
            var result = await _controller.BuyCompany(buyerCompanyId, companyId, percentage);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"company with id {buyerCompanyId} does not exist", notFoundResult.Value);
        }

        [Fact]
        public async Task BuyCompany_CompanyBought_ReturnsOkResult()
        {
            // Arrange
            var entityId = 1;
            var companyId = 2;
            var percentage = 50;
            var company = new CompanyModel(companyId, "Test Company");

            _mockCompanyService.Setup(s => s.GetByIdAsync(entityId)).Returns(Task.FromResult(company));
            _mockOwnershipService.Setup(s => s.BuyCompany(entityId, companyId, percentage)).Returns(Task.FromResult(new CompanyResponseDTO { Id=company.Id, Name= company.Name, AvailableShares = 50, WhoOwnsMe = company.WhoOwnsMeList}));

            // Act
            var result = await _controller.BuyCompany(entityId, companyId, percentage);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCompany = Assert.IsType<CompanyResponseDTO>(okResult.Value);
            Assert.Equal(companyId, returnedCompany.Id);
            Assert.Equal(company.Name, returnedCompany.Name);
        }
    }
}
