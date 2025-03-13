using Microsoft.AspNetCore.Mvc;

using UKParliament.CodeTest.Common.Entities;
using UKParliament.CodeTest.Common.Exceptions;
using UKParliament.CodeTest.Web.Models;
using UKParliament.CodeTest.Web.Services;

namespace UKParliament.CodeTest.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly PersonManagerApiService _personService;
        private readonly ILogger<HomeController> _logger;

        // Department cache (only loaded once)
        private static List<Department> _departmentsCache = new List<Department>();

        public HomeController(
            PersonManagerApiService personService,
            ILogger<HomeController> logger)
        {
            _personService = personService;
            _logger = logger;

            // Load departments only once
            if (!_departmentsCache.Any())
            {
                _departmentsCache = _personService.GetAllDepartments().ToList();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Retrieve people asynchronously
                var people = await _personService.GetAllPeopleAsync();

                // Map department names into Person list
                var departmentDict = _departmentsCache.ToDictionary(d => d.Id, d => d.Name);
                foreach (var person in people)
                {
                    if (departmentDict.ContainsKey(person.DepartmentId))
                    {
                        person.Department = new Department
                        {
                            Id = person.DepartmentId,
                            Name = departmentDict[person.DepartmentId]
                        };
                    }
                }

                var viewModel = new IndexViewModel
                {
                    People = people,
                    Departments = _departmentsCache
                };

                _logger.LogInformation("Loaded Index page with {PeopleCount} people and {DepartmentsCount} departments.",
                                       people.Count(), _departmentsCache.Count());

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading the Index page.");
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> SavePerson(Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided." });
            }

            Person updatedPerson;

            if (person.Id == 0)
            {
                // Add new person
                updatedPerson = await _personService.AddPersonAsync(person);
            }
            else
            {
                // Update existing person
                updatedPerson = await _personService.UpdatePersonAsync(person);
            }

            if (updatedPerson == null)
            {
                return BadRequest(new { message = "Error saving person." });
            }

            // Ensure Department is loaded from DB (prevents "Unknown")
            updatedPerson = await _personService.GetPersonByIdAsync(updatedPerson.Id);

            // Map department name using cached departments
            var departmentDict = _departmentsCache.ToDictionary(d => d.Id, d => d.Name);
            if (departmentDict.ContainsKey(updatedPerson.DepartmentId))
            {
                updatedPerson.Department = new Department
                {
                    Id = updatedPerson.DepartmentId,
                    Name = departmentDict[updatedPerson.DepartmentId]
                };
            }

            // Ensure JSON response includes full Department object
            return Json(new
            {
                id = updatedPerson.Id,
                firstName = updatedPerson.FirstName,
                lastName = updatedPerson.LastName,
                email = updatedPerson.Email,
                dateOfBirth = updatedPerson.DateOfBirth?.ToString("yyyy-MM-dd"),
                departmentId = updatedPerson.DepartmentId,
                departmentName = updatedPerson.Department?.Name ?? "Unknown"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                // The new method might throw PersonNotFoundException if not found, or simply succeed
                await _personService.DeletePersonAsync(id);

                _logger.LogInformation("Person with ID {Id} deleted successfully.", id);
                return RedirectToAction("Index");
            }
            catch (PersonNotFoundException nfex)
            {
                _logger.LogWarning(nfex, "Attempted to delete non-existing person with ID {Id}.", id);
                // Show error in the UI
                ModelState.AddModelError(string.Empty, nfex.Message);
                return await ReturnIndexViewWithData();
            }
            catch (Exception ex)
            {
                // Catch-all for unexpected errors
                _logger.LogError(ex, "Error deleting person with ID {Id}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Helper method to re-populate the Index view model in case of errors.
        /// </summary>
        private async Task<IActionResult> ReturnIndexViewWithData()
        {
            var people = await _personService.GetAllPeopleAsync();
            var departments = await Task.FromResult(_personService.GetAllDepartments()); // Avoid unnecessary Task.Run()

            // Map department names into Person list
            var departmentDict = departments.ToDictionary(d => d.Id, d => d.Name);
            foreach (var person in people)
            {
                if (departmentDict.ContainsKey(person.DepartmentId))
                {
                    person.Department = new Department
                    {
                        Id = person.DepartmentId,
                        Name = departmentDict[person.DepartmentId]
                    };
                }
            }

            var viewModel = new IndexViewModel
            {
                People = people,
                Departments = departments
            };

            return View("Index", viewModel);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
