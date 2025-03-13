
using System.Text.Json.Serialization;

namespace UKParliament.CodeTest.Common.Entities
{
    public class Department
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        // If you want the relationship to be navigable from Department's side:
        [JsonIgnore]
        public ICollection<Person>? Persons { get; set; }
    }
}
