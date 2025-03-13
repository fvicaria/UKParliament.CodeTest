using UKParliament.CodeTest.Common.Entities;


namespace UKParliament.CodeTest.Common.Interfaces
{
    public interface IDepartmentService
    {
        /// <summary>
        /// Retrieves all departments.
        /// </summary>
        IEnumerable<Department> GetAllDepartments();

        /// <summary>
        /// Retrieves a specific Department by its ID.
        /// </summary>
        Department? GetDepartmentById(int id);
    }

}