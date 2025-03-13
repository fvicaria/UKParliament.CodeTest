using Microsoft.Extensions.Logging;

using Moq;
using Xunit;

using UKParliament.CodeTest.Common.Interfaces;
using UKParliament.CodeTest.Services;
using UKParliament.CodeTest.Common.Entities;
using UKParliament.CodeTest.Common.Exceptions;


namespace UKParliament.CodeTest.Tests
{
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> _repositoryMock;
        private readonly Mock<ILogger<PersonService>> _loggerMock;
        private readonly PersonService _service;

        public PersonServiceTests()
        {
            _repositoryMock = new Mock<IPersonRepository>();
            _loggerMock = new Mock<ILogger<PersonService>>();

            // Create the service using the mocked repository and logger
            _service = new PersonService(_repositoryMock.Object, _loggerMock.Object);
        }

        #region GetAllPeopleAsync Tests

        [Fact]
        public async Task GetAllPeopleAsync_ReturnsListOfPeople()
        {
            // Arrange
            var people = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice@example.com",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    DepartmentId = 1
                },
                new Person
                {
                    Id = 2,
                    FirstName = "Bob",
                    LastName = "Jones",
                    Email = "bob@example.com",
                    DateOfBirth = new DateTime(1985, 12, 15),
                    DepartmentId = 2
                }
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(people);

            // Act
            var result = await _service.GetAllPeopleAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.FirstName == "Alice");
            Assert.Contains(result, p => p.FirstName == "Bob");
        }

        #endregion

        #region AddPersonAsync Tests

        [Fact]
        public async Task AddPersonAsync_ValidPerson_SavesAndReturnsPerson()
        {
            // Arrange
            var newPerson = new Person
            {
                FirstName = "Jane",
                LastName = "Doe",
                DepartmentId = 1,
                DateOfBirth = new DateTime(1990, 1, 1),
                Email = "jane@example.com"
            };

            // Mock repository to simulate setting a new ID on save
            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Person>()))
                .ReturnsAsync((Person p) =>
                {
                    p.Id = 123;
                    return p;
                });

            // Act
            var result = await _service.AddPersonAsync(newPerson);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(123, result.Id);
            Assert.Equal("Jane", result.FirstName);

            _repositoryMock.Verify(r => r.AddAsync(newPerson), Times.Once);
        }

        [Fact]
        public async Task AddPersonAsync_MissingFirstName_ThrowsValidationException()
        {
            // Arrange
            // Provide empty string to satisfy the 'required' property at compile-time
            // but trigger the PersonValidationException at runtime
            var invalidPerson = new Person
            {
                FirstName = "",  // intentionally empty
                LastName = "Doe",
                DepartmentId = 1,
                DateOfBirth = new DateTime(1980, 2, 2),
                Email = "test@example.com"
            };

            // Act & Assert
            await Assert.ThrowsAsync<PersonValidationException>(() => _service.AddPersonAsync(invalidPerson));

            // Ensure no database write attempts were made
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
        }

        #endregion

        #region UpdatePersonAsync Tests

        [Fact]
        public async Task UpdatePersonAsync_ExistingPerson_UpdatesAndReturnsPerson()
        {
            // Arrange
            var existingPerson = new Person
            {
                Id = 999,
                FirstName = "John",
                LastName = "Doe",
                DepartmentId = 2,
                DateOfBirth = new DateTime(1970, 1, 1),
                Email = "john@example.com"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(999))
                           .ReturnsAsync(existingPerson);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Person>()))
                           .ReturnsAsync((Person p) => p);

            // Act
            var updatedPerson = await _service.UpdatePersonAsync(existingPerson);

            // Assert
            Assert.NotNull(updatedPerson);
            Assert.Equal(999, updatedPerson.Id);
            _repositoryMock.Verify(r => r.UpdateAsync(existingPerson), Times.Once);
        }

        [Fact]
        public async Task UpdatePersonAsync_NotFound_ThrowsPersonNotFoundException()
        {
            // Arrange
            var personToUpdate = new Person
            {
                Id = 1234,
                FirstName = "DoesNot",
                LastName = "Exist",
                DepartmentId = 2,
                DateOfBirth = new DateTime(1990, 5, 5),
                Email = "doesnotexist@example.com"
            };

            // Simulate the repository returning null if not found
            _repositoryMock.Setup(r => r.GetByIdAsync(1234))
                           .ReturnsAsync((Person)null);

            // Act & Assert
            await Assert.ThrowsAsync<PersonNotFoundException>(() => _service.UpdatePersonAsync(personToUpdate));

            // Ensure no update was attempted
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Person>()), Times.Never);
        }

        #endregion

        #region DeletePersonAsync Tests

        [Fact]
        public async Task DeletePersonAsync_PersonExists_DeletesSuccessfully()
        {
            // Arrange
            var existingPerson = new Person
            {
                Id = 100,
                FirstName = "Test",
                LastName = "Dummy",
                Email = "dummy@example.com",
                DateOfBirth = new DateTime(1970, 1, 1),
                DepartmentId = 1
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(100))
                           .ReturnsAsync(existingPerson);

            // Act
            await _service.DeletePersonAsync(100);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(100), Times.Once);
        }

        [Fact]
        public async Task DeletePersonAsync_PersonNotFound_ThrowsPersonNotFoundException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((Person)null);

            // Act & Assert
            await Assert.ThrowsAsync<PersonNotFoundException>(() => _service.DeletePersonAsync(999));

            // No deletion should be attempted
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion

        #region GetPersonByIdAsync Tests

        [Fact]
        public async Task GetPersonByIdAsync_Found_ReturnsPerson()
        {
            // Arrange
            var person = new Person
            {
                Id = 456,
                FirstName = "Bob",
                LastName = "Baker",
                Email = "bob@example.com",
                DateOfBirth = new DateTime(1995, 1, 1),
                DepartmentId = 2
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(456)).ReturnsAsync(person);

            // Act
            var result = await _service.GetPersonByIdAsync(456);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bob", result.FirstName);
        }

        [Fact]
        public async Task GetPersonByIdAsync_NotFound_ThrowsPersonNotFoundException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Person)null);

            // Act & Assert
            await Assert.ThrowsAsync<PersonNotFoundException>(() => _service.GetPersonByIdAsync(999));
        }

        #endregion
    }
}
