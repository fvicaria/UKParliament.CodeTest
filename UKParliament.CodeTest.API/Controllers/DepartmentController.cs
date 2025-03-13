
using Microsoft.AspNetCore.Mvc;

using UKParliament.CodeTest.Common.Interfaces;
using UKParliament.CodeTest.Common.Entities;


namespace UKParliament.CodeTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: api/department
        [HttpGet]
        public ActionResult<IEnumerable<Department>> GetAll()
        {
            try
            {
                var departments = _departmentService.GetAllDepartments();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/department/5
        [HttpGet("{id}")]
        public ActionResult<Department> GetById(int id)
        {
            try
            {
                var department = _departmentService.GetDepartmentById(id);
                if (department == null)
                {
                    return NotFound($"Department with ID {id} not found.");
                }
                return Ok(department);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
