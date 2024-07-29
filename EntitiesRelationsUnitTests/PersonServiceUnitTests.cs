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
    public class PersonServiceUnitTests
    {
        private readonly Mock<IPersonProvider> _mockPersonProvider;
        private readonly Mock<ICompanyProvider> _mockCompanyProvider;
        private readonly PersonService _personService;
        public PersonServiceUnitTests()
        {
            _mockPersonProvider = new Mock<IPersonProvider>();
            _mockCompanyProvider = new Mock<ICompanyProvider>();
            _personService = new PersonService(_mockPersonProvider.Object, _mockCompanyProvider.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenPersonProviderIsNull()
        {
            //Arrange + Act + Assert
            Assert.Throws<ArgumentNullException>(() => new PersonService(null, _mockCompanyProvider.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenCompanyProviderIsNull()
        {
            //Arrange + Act + Assert
            Assert.Throws<ArgumentNullException>(() => new PersonService(_mockPersonProvider.Object, null));
        }

        [Fact]
        public void Constructor_InitializesWithValidProviders()
        {
            // Arrange + Act
            var service = new PersonService(_mockPersonProvider.Object, _mockCompanyProvider.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public async Task AddPerson_PersonAndCompanyDoNotExist_ReturnsTrue()
        {
            // Arrange
            var personRequest = new PersonRequestDTO { Id = 1, Name = "John Doe" };
            _mockPersonProvider.Setup(p => p.GetPersonById(personRequest.Id)).Returns(Task.FromResult<PersonModel>(null));
            _mockCompanyProvider.Setup(c => c.GetCompanyById(personRequest.Id)).Returns(Task.FromResult<CompanyModel>(null));

            // Act
            var result = await _personService.AddPerson(personRequest);

            // Assert
            Assert.True(result);
            _mockPersonProvider.Verify(p => p.AddPerson(It.Is<PersonModel>(pm => pm.Id == personRequest.Id && pm.Name == personRequest.Name)), Times.Once);
        }

        [Fact]
        public async Task AddPerson_PersonAlreadyExists_ReturnsFalse()
        {
            // Arrange
            var personRequest = new PersonRequestDTO { Id = 1, Name = "John Doe" };
            var existingPerson = new PersonModel(personRequest.Id, personRequest.Name, new List<long>());
            _mockPersonProvider.Setup(p => p.GetPersonById(personRequest.Id)).Returns(Task.FromResult(existingPerson));

            // Act
            var result = await _personService.AddPerson(personRequest);

            // Assert
            Assert.False(result);
            _mockPersonProvider.Verify(p => p.AddPerson(It.IsAny<PersonModel>()), Times.Never);
        }

        [Fact]
        public async Task AddPerson_CompanyAlreadyExists_ReturnsFalse()
        {
            // Arrange
            var personRequest = new PersonRequestDTO { Id = 1, Name = "John Doe" };
            var existingCompany = new CompanyModel(personRequest.Id, "company1");
            _mockPersonProvider.Setup(p => p.GetPersonById(personRequest.Id)).Returns(Task.FromResult<PersonModel>(null));
            _mockCompanyProvider.Setup(c => c.GetCompanyById(personRequest.Id)).Returns(Task.FromResult(existingCompany));

            // Act
            var result = await _personService.AddPerson(personRequest);

            // Assert
            Assert.False(result);
            _mockPersonProvider.Verify(p => p.AddPerson(It.IsAny<PersonModel>()), Times.Never);
        }

        [Fact]
        public async Task GetAllPersons_ReturnsAllPersons()
        {
            // Arrange
            var persons = new List<PersonModel>
            {
                new PersonModel(1, "person1", new List<long>()),
                new PersonModel(2, "person2", new List<long>())
            };

            _mockPersonProvider.Setup(p => p.GetAllPersons()).ReturnsAsync(persons);

            // Act
            var result = await _personService.GetAllPersons();

            // Assert
            Assert.Equal(persons, result);
            _mockPersonProvider.Verify(p => p.GetAllPersons(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_PersonExists_ReturnsPerson()
        {
            // Arrange
            var personId = 1;
            var person = new PersonModel(personId, "person1", new List<long>());

            _mockPersonProvider.Setup(p => p.GetPersonById(personId)).ReturnsAsync(person);

            // Act
            var result = await _personService.GetByIdAsync(personId);

            // Assert
            Assert.Equal(person, result);
            _mockPersonProvider.Verify(p => p.GetPersonById(personId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_PersonDoesNotExist_ReturnsNull()
        {
            // Arrange
            var personId = 1;

            _mockPersonProvider.Setup(p => p.GetPersonById(personId)).ReturnsAsync((PersonModel)null);

            // Act
            var result = await _personService.GetByIdAsync(personId);

            // Assert
            Assert.Null(result);
            _mockPersonProvider.Verify(p => p.GetPersonById(personId), Times.Once);
        }

        [Fact]
        public async Task UpdatePerson_PersonExists_ReturnsUpdatedPerson()
        {
            // Arrange
            var person = new PersonModel(1, "person1", new List<long>());
            var updatedPerson = new PersonModel(1, "person2", new List<long>());

            _mockPersonProvider.Setup(p => p.UpdatePerson(person)).ReturnsAsync(updatedPerson);

            // Act
            var result = await _personService.UpdatePerson(person);

            // Assert
            Assert.Equal(updatedPerson, result);
            _mockPersonProvider.Verify(p => p.UpdatePerson(person), Times.Once);
        }
    }
}
