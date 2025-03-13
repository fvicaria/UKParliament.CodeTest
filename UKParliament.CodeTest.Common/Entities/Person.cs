
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UKParliament.CodeTest.Common.Entities
{
    public class Person
    {
        public int Id { get; set; }

        [StringLength(50)]
        public required string FirstName { get; set; }

        [StringLength(50)]
        public required string LastName { get; set; }

        [DataType(DataType.Date)]
        public required DateTime? DateOfBirth { get; set; }

        // Foreign key to Department
        public required int DepartmentId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }

        [StringLength(100)]
        public required string Email { get; set; }
    }
}