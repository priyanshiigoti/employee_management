using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace employee_management.Models.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
       [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com)$", ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Incorrect Phone number")]
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("DepartmentId")]
        public Guid DepartmentId{ get; set; }
        public Department? department {  get; set; }
    }
}
