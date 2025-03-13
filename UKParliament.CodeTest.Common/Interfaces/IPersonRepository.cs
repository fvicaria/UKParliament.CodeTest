
using UKParliament.CodeTest.Common.Entities;

namespace UKParliament.CodeTest.Common.Interfaces
{
    /// <summary>
    /// Defines a repository contract for managing <see cref="Person"/> entities.
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Retrieves all <see cref="Person"/> records, including any relevant navigation data.
        /// </summary>
        Task<IEnumerable<Person>> GetAllAsync();

        /// <summary>
        /// Retrieves a single <see cref="Person"/> by its unique ID.
        /// Returns <c>null</c> if no person is found.
        /// </summary>
        Task<Person?> GetByIdAsync(int id);

        /// <summary>
        /// Persists a new <see cref="Person"/> to the data store.
        /// </summary>
        /// <param name="person">The <see cref="Person"/> to add.</param>
        /// <returns>The newly added <see cref="Person"/>, including its assigned ID.</returns>
        Task<Person> AddAsync(Person person);

        /// <summary>
        /// Updates an existing <see cref="Person"/> in the data store.
        /// </summary>
        /// <param name="person">The <see cref="Person"/> with updated fields.</param>
        /// <returns>The updated <see cref="Person"/>.</returns>
        Task<Person?> UpdateAsync(Person person);

        /// <summary>
        /// Deletes an existing <see cref="Person"/> by its unique ID.
        /// </summary>
        /// <param name="id">The person's unique identifier.</param>
        Task<bool> DeleteAsync(int id);
    }
}
