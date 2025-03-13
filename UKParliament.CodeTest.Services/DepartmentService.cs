
using UKParliament.CodeTest.Common.Entities;
using UKParliament.CodeTest.Common.Interfaces;
using UKParliament.CodeTest.Data;

namespace UKParliament.CodeTest.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly PersonManagerContext _context;

        public DepartmentService(PersonManagerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all Departments in alphabetical order by Name (or your choice).
        /// </summary>
        public IEnumerable<Department> GetAllDepartments()
        {
            return _context.Departments
                           .OrderBy(d => d.Id)
                           .ToList();
        }

        /// <summary>
        /// Retrieves a Department by its ID. Returns null if not found.
        /// </summary>
        public Department? GetDepartmentById(int id)
        {
            return _context.Departments.Find(id);
        }
    }
}
