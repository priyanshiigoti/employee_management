using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace employee_management.Models.Entities
{
    public class Employee
    {
       
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public bool IsActive { get; set; }
            public Guid DepartmentId { get; set; }

            [ForeignKey("DepartmentId")]
            public Department? department { get; set; }
        

    }
}
