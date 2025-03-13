using Microsoft.AspNetCore.Mvc;
using UKParliament.CodeTest.Common.Interfaces;
using UKParliament.CodeTest.Web.Services;


namespace UKParliament.CodeTest.Web.Controllers
{
    public class PersonController : Controller
    {
        private readonly PersonManagerApiService _personService;

        public PersonController(PersonManagerApiService personService)
        {
            _personService = personService;
        }

        public async Task<IActionResult> Index()
        {
            var people = await _personService.GetAllPeopleAsync();
            return View(people);
        }
    }
}
