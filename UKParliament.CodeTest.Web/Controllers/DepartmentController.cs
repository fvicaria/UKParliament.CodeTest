using Microsoft.AspNetCore.Mvc;
using UKParliament.CodeTest.Common.Interfaces;
using UKParliament.CodeTest.Web.Services;


namespace UKParliament.CodeTest.Web2.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly PersonManagerApiService _departmentService;

        public DepartmentController(PersonManagerApiService departmentService)
        {
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            var departments = _departmentService.GetAllDepartments();
            return View(departments);
        }
    }
}
