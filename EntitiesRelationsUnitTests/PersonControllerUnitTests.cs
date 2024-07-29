using AutoMapper;
using EntitiesRelations.API.Controllers;
using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;
using EntitiesRelations.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;

namespace EntitiesRelationsUnitTests
{
    public class PersonControllerUnitTests
    {
        private Mock<IPersonService> _mockPersonService;
        private Mock<IOwnershipService> _mockOwnershipService;
        private readonly PersonController _controller;

        public PersonControllerUnitTests()
        {
            _mockPersonService = new Mock<IPersonService>();
            _mockOwnershipService = new Mock<IOwnershipService>();
            _controller = new PersonController(_mockPersonService.Object, _mockOwnershipService.Object);
        }


        [Fact]
        public void PersonControllerConstructor_ShouldThrowException_WhenPersonServiceIsNull()
        {
            //Arrange + Act + Assert
            Assert.Throws<ArgumentNullException>(() => new PersonController(null, _mockOwnershipService.Object));
        }

        [Fact]
        public void PersonControllerConstructor_ShouldThrowException_WhenOwnershipServiceIsNull()
        {
            //Arrange + Act + Assert
            Assert.Throws<ArgumentNullException>(() => new PersonController(_mockPersonService.Object, null));
        }

        [Fact]
        public async Task GetShouldReturnOkObjectResult()
        {
            // Arrange
            var persons = new List<PersonModel> 
            { 
                new PersonModel(1234, "Person1",null), 
                new PersonModel(1235,"Person2",null)
            };
            _mockPersonService.Setup(service => service.GetAllPersons()).ReturnsAsync(persons);

            //Act
            var systemUnderTest = await _controller.GetAsync();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(systemUnderTest);
            var returnValue = Assert.IsType<List<PersonResponseDTO>>(okResult.Value);
            Assert.Equal(persons.Count, returnValue.Count);
        }
        [Fact]
        public async Task GetAsync_ReturnsOkResult_WithPerson()
        {
            // Arrange
            var personId = 1234;
            var person = new PersonModel ( personId,  "Person1", null );
            _mockPersonService.Setup(service => service.GetByIdAsync(personId)).ReturnsAsync(person);

            // Act
            var systemUnderTest = await _controller.GetAsync(personId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(systemUnderTest);
            var returnValue = Assert.IsType<PersonResponseDTO>(okResult.Value);
            Assert.Equal(personId, returnValue.Id);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound_WhenPersonIsNotFound()
        {
            // Arrange
            var personId = 1234;
            _mockPersonService.Setup(service => service.GetByIdAsync(personId)).ReturnsAsync((PersonModel)null);

            // Act
            var systemUnderTest = await _controller.GetAsync(personId);

            // Assert
            Assert.IsType<NotFoundResult>(systemUnderTest);
        }


        [Fact]
        public async Task Put_PersonFound_ReturnsOkResult()
        {
            // Arrange
            var personRequest = new PersonRequestDTO { Id = 1234, Name = "Person Name" };
            var person = new PersonModel(1234,"Person Name", null );
            var updatedPerson = new PersonModel(1234, "Updated Name", null);
            var task = Task.FromResult(person);
            var updateTask = Task.FromResult(updatedPerson);

            _mockPersonService.Setup(s => s.GetByIdAsync(personRequest.Id)).Returns(task);
            _mockPersonService.Setup(s => s.UpdatePerson(person)).Returns(updateTask);

            // Act
            var result = await _controller.Put(personRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PersonResponseDTO>(okResult.Value);
            Assert.Equal(updatedPerson.Id, returnValue.Id);
            Assert.Equal(updatedPerson.Name, returnValue.Name);
        }

        [Fact]
        public async Task Put_PersonNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var personRequest = new PersonRequestDTO { Id = 1, Name = "Non existant Person" };

            _mockPersonService.Setup(s => s.GetByIdAsync(personRequest.Id)).Returns(Task.FromResult<PersonModel>(null));

            // Act
            var result = await _controller.Put(personRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Could not find person with id {personRequest.Id}", notFoundResult.Value);
        }


        [Fact]
        public async Task Delete_PersonFound_ReturnsOkResult()
        {
            // Arrange
            var personId = 1;
            var person = new PersonModel(personId, "Deleted Person", null);
            var task = Task.FromResult(person);

            _mockPersonService.Setup(s => s.GetByIdAsync(personId)).Returns(task);
            _mockPersonService.Setup(s => s.DeletePerson(personId)).Verifiable();

            // Act
            var result = await _controller.Delete(personId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"person with id {personId} was successfully deleted.", okResult.Value);

            _mockPersonService.Verify(s => s.DeletePerson(personId), Times.Once);
        }

        [Fact]
        public async Task Delete_PersonNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var personId = 1;

            _mockPersonService.Setup(s => s.GetByIdAsync(personId)).Returns(Task.FromResult<PersonModel>(null));

            // Act
            var result = await _controller.Delete(personId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AssignRelations_Person1NotFound_ReturnsNotFound()
        {
            // Arrange
            var person1Id = 1;
            var person2Id = 2;

            _mockPersonService.Setup(s => s.GetByIdAsync(person1Id)).Returns(Task.FromResult<PersonModel>(null));
            _mockPersonService.Setup(s => s.GetByIdAsync(person2Id)).Returns(Task.FromResult(new PersonModel(person2Id, "Some Name", null )));

            // Act
            var result = await _controller.AssignRelations(person1Id, person2Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Could not find person with id {person1Id}", notFoundResult.Value);
        }

        [Fact]
        public async Task AssignRelations_Person2NotFound_ReturnsNotFound()
        {
            // Arrange
            var person1Id = 1;
            var person2Id = 2;

            _mockPersonService.Setup(s => s.GetByIdAsync(person1Id)).Returns(Task.FromResult(new PersonModel(person1Id, "Some Name", null)));
            _mockPersonService.Setup(s => s.GetByIdAsync(person2Id)).Returns(Task.FromResult<PersonModel>(null));

            // Act
            var result = await _controller.AssignRelations(person1Id, person2Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Could not find person with id {person2Id}", notFoundResult.Value);
        }

        [Fact]
        public async Task AssignRelations_RelationshipEstablished_ReturnsOkWithPersons()
        {
            // Arrange
            var person1Id = 1;
            var person2Id = 2;
            var person1 = new PersonModel(person1Id, "Person1", new List<long>());
            var person2 = new PersonModel(person2Id, "Person2", new List<long>());

            _mockPersonService.Setup(s => s.GetByIdAsync(person1Id)).Returns(Task.FromResult(person1));
            _mockPersonService.Setup(s => s.GetByIdAsync(person2Id)).Returns(Task.FromResult(person2));
            _mockPersonService.Setup(s => s.EstablishRelationships(person1, person2)).Returns(Task.FromResult(true));

            // Act
            var result = await _controller.AssignRelations(person1Id, person2Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PersonResponseDTO>>(okResult.Value);
            Assert.Contains(returnValue, p => p.Id == person1Id);
            Assert.Contains(returnValue, p => p.Id == person2Id);
        }
        [Fact]
        public async Task AssignRelations_RelationshipAlreadyExists_ReturnsOkWithMessage()
        {
            // Arrange
            var person1Id = 1;
            var person2Id = 2;
            var person1 = new PersonModel(person1Id, "Person1", new List<long>{ person2Id});
            var person2 = new PersonModel(person2Id, "Person2", new List<long> { person1Id} );

            _mockPersonService.Setup(s => s.GetByIdAsync(person1Id)).Returns(Task.FromResult(person1));
            _mockPersonService.Setup(s => s.GetByIdAsync(person2Id)).Returns(Task.FromResult(person2));
            _mockPersonService.Setup(s => s.EstablishRelationships(person1, person2)).Returns(Task.FromResult(false));

            // Act
            var result = await _controller.AssignRelations(person1Id, person2Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Relationship between {person1Id} and {person2Id} was already set", okResult.Value);
        }

        [Fact]
        public async Task DeleteRelations_Person1NotFound_ReturnsNotFound()
        {
            // Arrange
            var person1Id = 1;
            var person2Id = 2;

            _mockPersonService.Setup(s => s.GetByIdAsync(person1Id)).Returns(Task.FromResult<PersonModel>(null));
            _mockPersonService.Setup(s => s.GetByIdAsync(person2Id)).Returns(Task.FromResult(new PersonModel(person1Id, "Person1", null)));

            // Act
            var result = await _controller.DeleteRelations(person1Id, person2Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Could not find person with id {person1Id}", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteRelations_Person2NotFound_ReturnsNotFound()
        {
            // Arrange
            var person1Id = 1;
            var person2Id = 2;

            _mockPersonService.Setup(s => s.GetByIdAsync(person1Id)).Returns(Task.FromResult(new PersonModel(person1Id, "Person1", null)));
            _mockPersonService.Setup(s => s.GetByIdAsync(person2Id)).Returns(Task.FromResult<PersonModel>(null));

            // Act
            var result = await _controller.DeleteRelations(person1Id, person2Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Could not find person with id {person2Id}", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteRelations_RelationshipDeleted_ReturnsOkWithPersons()
        {
            // Arrange
            var person1Id = 1;
            var person2Id = 2;
            var person1 = new PersonModel(person1Id, "Person1", new List<long> { person2Id });
            var person2 = new PersonModel(person2Id, "Person2", new List<long> { person1Id });

            _mockPersonService.Setup(s => s.GetByIdAsync(person1Id)).Returns(Task.FromResult(person1));
            _mockPersonService.Setup(s => s.GetByIdAsync(person2Id)).Returns(Task.FromResult(person2));
            _mockPersonService.Setup(s => s.DeleteRelationships(person1, person2)).Returns(Task.FromResult(true));

            // Act
            var result = await _controller.DeleteRelations(person1Id, person2Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PersonModel>>(okResult.Value);
            Assert.Contains(returnValue, p => p.Id == person1Id);
            Assert.Contains(returnValue, p => p.Id == person2Id);
        }

        [Fact]
        public async Task DeleteRelations_RelationshipAlreadyNonExistent_ReturnsOkWithMessage()
        {
            // Arrange
            var person1Id = 1;
            var person2Id = 2;
            var person1 = new PersonModel(person1Id, "Person1", new List<long>());
            var person2 = new PersonModel(person2Id, "Person2", new List<long>());

            _mockPersonService.Setup(s => s.GetByIdAsync(person1Id)).Returns(Task.FromResult(person1));
            _mockPersonService.Setup(s => s.GetByIdAsync(person2Id)).Returns(Task.FromResult(person2));
            _mockPersonService.Setup(s => s.DeleteRelationships(person1, person2)).Returns(Task.FromResult(false));

            // Act
            var result = await _controller.DeleteRelations(person1Id, person2Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Relationship between {person1Id} and {person2Id} already non existent", okResult.Value);
        }
    }
}
