
using UKParliament.CodeTest.Common.Entities;
using UKParliament.CodeTest.Common.Interfaces;


namespace UKParliament.CodeTest.Web.Services
{
    public class PersonManagerApiService : IPersonService, IDepartmentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PersonManagerApiService> _logger;

        public PersonManagerApiService(HttpClient httpClient, ILogger<PersonManagerApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Person>> GetAllPeopleAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Person>>("person")
                    ?? new List<Person>(); // Ensure we return an empty list if deserialization fails
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve people from the API.");
                return new List<Person>();
            }
        }

        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Person>($"person/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve person with ID {PersonId}.", id);
                return null;
            }
        }

        public async Task<Person?> AddPersonAsync(Person person)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("person", person);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to add person. Status code: {StatusCode}", response.StatusCode);
                    return null; // Return null if the request fails
                }

                return await response.Content.ReadFromJsonAsync<Person>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding person to the API.");
                return null;
            }
        }

        public async Task<Person?> UpdatePersonAsync(Person person)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"person/{person.Id}", person);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to update person with ID {PersonId}. Status code: {StatusCode}", person.Id, response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<Person>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating person with ID {PersonId}.", person.Id);
                return null;
            }
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"person/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to delete person with ID {PersonId}. Status code: {StatusCode}", id, response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting person with ID {PersonId}.", id);
                return false;
            }
        }

        // ------------------- DEPARTMENT METHODS -------------------

        public IEnumerable<Department> GetAllDepartments()
        {
            try
            {
                var task = _httpClient.GetFromJsonAsync<IEnumerable<Department>>("department");
                task.Wait(); // Blocking call to get result synchronously
                return task.Result ?? new List<Department>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve departments from the API.");
                return new List<Department>();
            }
        }

        public Department? GetDepartmentById(int id)
        {
            try
            {
                var task = _httpClient.GetFromJsonAsync<Department>($"department/{id}");
                task.Wait(); // Blocking call to get result synchronously
                return task.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve department with ID {DepartmentId}.", id);
                return null;
            }
        }
    }
}
