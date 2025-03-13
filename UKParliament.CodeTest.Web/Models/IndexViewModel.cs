
using UKParliament.CodeTest.Common.Entities;

namespace UKParliament.CodeTest.Web.Models;

public class IndexViewModel
{
    public IEnumerable<Person> People { get; set; } = new List<Person>();
    public IEnumerable<Department> Departments { get; set; } = new List<Department>();
}