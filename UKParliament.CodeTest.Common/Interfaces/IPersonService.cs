using UKParliament.CodeTest.Common.Entities;


namespace UKParliament.CodeTest.Common.Interfaces
{
    public interface IPersonService
    {
        /// <summary>
        /// Retrieves all <see cref="Person"/> entities.
        /// </summary>
        /// <returns>A collection of <see cref="Person"/>.</returns>
        Task<IEnumerable<Person>> GetAllPeopleAsync();

        /// <summary>
        /// Retrieves a single <see cref="Person"/> by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the person.</param>
        /// <returns>The matching <see cref="Person"/> if found.</returns>
        /// <exception cref="PersonNotFoundException">Thrown if the <see cref="Person"/> does not exist.</exception>
        Task<Person?> GetPersonByIdAsync(int id);

        /// <summary>
        /// Creates a new <see cref="Person"/> record in the database.
        /// </summary>
        /// <param name="person">The <see cref="Person"/> entity to add.</param>
        /// <returns>The newly added <see cref="Person"/>, including its assigned ID.</returns>
        /// <exception cref="PersonValidationException">Thrown if validation fails (required fields are missing).</exception>
        Task<Person?> AddPersonAsync(Person person);

        /// <summary>
        /// Updates an existing <see cref="Person"/> record in the database.
        /// </summary>
        /// <param name="person">The <see cref="Person"/> entity with updated fields.</param>
        /// <returns>The updated <see cref="Person"/>.</returns>
        /// <exception cref="PersonNotFoundException">Thrown if the <see cref="Person"/> does not exist.</exception>
        /// <exception cref="PersonValidationException">Thrown if validation fails.</exception>
        Task<Person?> UpdatePersonAsync(Person person);

        /// <summary>
        /// Deletes an existing <see cref="Person"/> by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the person.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="PersonNotFoundException">Thrown if the <see cref="Person"/> does not exist.</exception>
        Task<bool> DeletePersonAsync(int id);
    }
}
