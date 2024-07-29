using EntitiesRelations.API.Controllers;
using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;
using EntitiesRelations.API.Providers;
using EntitiesRelations.API.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesRelationsUnitTests
{
    public class CompanyServiceUnitTests
    {
        private readonly Mock<ICompanyProvider> _mockCompanyProvider;
        private readonly Mock<IPersonProvider> _mockPersonProvider;
        private readonly CompanyService _service;

        public CompanyServiceUnitTests()
        {
            _mockCompanyProvider = new Mock<ICompanyProvider>();
            _mockPersonProvider = new Mock<IPersonProvider>();
            _service = new CompanyService(_mockCompanyProvider.Object, _mockPersonProvider.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenCompanyProviderIsNull()
        {
            //Arrange + Act + Assert
            Assert.Throws<ArgumentNullException>(() => new CompanyService(null, _mockPersonProvider.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenPersonProviderIsNull()
        {
            //Arrange + Act + Assert
            Assert.Throws<ArgumentNullException>(() => new CompanyService(_mockCompanyProvider.Object, null));
        }

        [Fact]
        public void Constructor_InitializesWithValidProviders()
        {
            // Act + Arrange
            var service = new CompanyService(_mockCompanyProvider.Object, _mockPersonProvider.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public async Task AddCompany_ReturnsTrue_WhenCompanyAndPersonDoNotExist()
        {
            // Arrange
            var companyDTO = new CompanyRequestDTO { Id = 1, Name = "Test Company" };
            _mockCompanyProvider.Setup(p => p.GetCompanyById(companyDTO.Id)).ReturnsAsync((CompanyModel)null);
            _mockPersonProvider.Setup(p => p.GetPersonById(companyDTO.Id)).ReturnsAsync((PersonModel)null);

            // Act
            var result = await _service.AddCompany(companyDTO);

            // Assert
            Assert.True(result);
            _mockCompanyProvider.Verify(p => p.AddCompany(It.IsAny<CompanyModel>()), Times.Once);
        }

        [Fact]
        public async Task AddCompany_ReturnsFalse_WhenCompanyExists()
        {
            // Arrange
            var companyDTO = new CompanyRequestDTO { Id = 1, Name = "Test Company" };
            var existingCompany = new CompanyModel(companyDTO.Id, companyDTO.Name);
            _mockCompanyProvider.Setup(p => p.GetCompanyById(companyDTO.Id)).ReturnsAsync(existingCompany);
            _mockPersonProvider.Setup(p => p.GetPersonById(companyDTO.Id)).ReturnsAsync((PersonModel)null);

            // Act
            var result = await _service.AddCompany(companyDTO);

            // Assert
            Assert.False(result);
            _mockCompanyProvider.Verify(p => p.AddCompany(It.IsAny<CompanyModel>()), Times.Never);
        }

        [Fact]
        public async Task AddCompany_ReturnsFalse_WhenPersonWithSameIdExists()
        {
            // Arrange
            var companyDTO = new CompanyRequestDTO { Id = 1, Name = "Test Company" };
            var existingPerson = new PersonModel(companyDTO.Id, "Test Person", null);
            _mockCompanyProvider.Setup(p => p.GetCompanyById(companyDTO.Id)).ReturnsAsync((CompanyModel)null);
            _mockPersonProvider.Setup(p => p.GetPersonById(companyDTO.Id)).ReturnsAsync(existingPerson);

            // Act
            var result = await _service.AddCompany(companyDTO);

            // Assert
            Assert.False(result);
            _mockCompanyProvider.Verify(p => p.AddCompany(It.IsAny<CompanyModel>()), Times.Never);
        }

        [Fact]
        public async Task GetAllCompanies_ReturnsListOfCompanies()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel (1, "Company A" ),
                new CompanyModel (2, "Company B" ),
            };
            _mockCompanyProvider.Setup(provider => provider.GetAllCompanies()).ReturnsAsync(companies);

            // Act
            var result = await _service.GetAllCompanies();

            // Assert
            Assert.Equal(companies, result);
        }

        [Fact]
        public async Task UpdateCompany_ReturnsUpdatedCompany()
        {
            // Arrange
            var companyDTO = new CompanyRequestDTO { Id = 1, Name = "Updated Company" };
            var companyModel = new CompanyModel(companyDTO.Id, companyDTO.Name);
            _mockCompanyProvider.Setup(provider => provider.UpdateCompany(It.Is<CompanyModel>(c => c.Id == companyDTO.Id && c.Name == companyDTO.Name)))
                                .ReturnsAsync(companyModel);

            // Act
            var result = await _service.UpdateCompany(companyModel);

            // Assert
            Assert.Equal(companyModel, result);
            _mockCompanyProvider.Verify(provider => provider.UpdateCompany(It.Is<CompanyModel>(c => c.Id == companyDTO.Id && c.Name == companyDTO.Name)), Times.Once);
        }

        [Fact]
        public async Task DeleteCompany_CallsProviderDeleteCompany()
        {
            // Arrange
            var companyId = 1L;
            _mockCompanyProvider.Setup(provider => provider.DeleteCompany(companyId)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteCompany(companyId);

            // Assert
            _mockCompanyProvider.Verify(provider => provider.DeleteCompany(companyId), Times.Once);
        }
    }
}
