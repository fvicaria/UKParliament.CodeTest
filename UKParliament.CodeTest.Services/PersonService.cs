
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using UKParliament.CodeTest.Common.Exceptions;
using UKParliament.CodeTest.Common.Interfaces;
using UKParliament.CodeTest.Common.Entities;


namespace UKParliament.CodeTest.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IPersonRepository personRepository, ILogger<PersonService> logger)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Person>> GetAllPeopleAsync()
        {
            _logger.LogInformation("Retrieving all people from the database.");
            var people = await _personRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} people from the database.", (people as ICollection<Person>)?.Count ?? 0);
            return people;
        }

        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            _logger.LogInformation("Fetching person with ID {PersonId}.", id);
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null)
            {
                _logger.LogWarning("Person with ID {PersonId} not found.", id);
                throw new PersonNotFoundException($"Person with ID {id} was not found.");
            }
            return person;
        }

        public async Task<Person?> AddPersonAsync(Person person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));

            _logger.LogInformation("Adding new person: {@Person}.", person);
            ValidatePerson(person); // Validate input fields (throws if invalid)

            try
            {
                var addedPerson = await _personRepository.AddAsync(person);
                _logger.LogInformation("Person added successfully with ID {PersonId}.", addedPerson.Id);
                return addedPerson;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while adding person {@Person}.", person);
                throw new PersonUpdateException("Failed to add person due to a database error.", ex);
            }
        }

        public async Task<Person?> UpdatePersonAsync(Person person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));

            _logger.LogInformation("Updating person with ID {PersonId}.", person.Id);
            ValidatePerson(person); // Validate input fields

            var existingPerson = await _personRepository.GetByIdAsync(person.Id);
            if (existingPerson == null)
            {
                _logger.LogWarning("Person with ID {PersonId} not found for update.", person.Id);
                throw new PersonNotFoundException($"Failed to update person with ID {person.Id}. Person not found.");
            }

            try
            {
                var updatePerson = await _personRepository.UpdateAsync(person);
                _logger.LogInformation("Person with ID {PersonId} updated successfully.", person.Id);
                return updatePerson; // Return true if update succeeds
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict when updating person with ID {PersonId}.", person.Id);
                throw new PersonUpdateException($"Failed to update person with ID {person.Id} due to a concurrency issue.", ex);
            }
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            _logger.LogInformation("Deleting person with ID {PersonId}.", id);
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null)
            {
                _logger.LogWarning("Person with ID {PersonId} not found for deletion.", id);
                throw new PersonNotFoundException($"Person with ID {id} was not found.");
            }

            try
            {
                bool deleted = await _personRepository.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Person with ID {PersonId} deleted successfully.", id);
                }
                return deleted;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict when deleting person with ID {PersonId}.", id);
                throw new PersonUpdateException($"Failed to delete person with ID {id} due to a concurrency issue.", ex);
            }
        }

        /// <summary>
        /// Validates the Person object for required fields. 
        /// Throws PersonValidationException if validation fails.
        /// </summary>
        private void ValidatePerson(Person person)
        {
            if (string.IsNullOrWhiteSpace(person.FirstName))
                throw new PersonValidationException("First name is required.");
            if (string.IsNullOrWhiteSpace(person.LastName))
                throw new PersonValidationException("Last name is required.");
            if (string.IsNullOrWhiteSpace(person.Email))
                throw new PersonValidationException("Email is required.");
            if (person.DateOfBirth == null)
                throw new PersonValidationException("Date of birth is required.");
            if (person.DepartmentId <= 0)
                throw new PersonValidationException("A valid DepartmentId is required.");
        }
    }
}
