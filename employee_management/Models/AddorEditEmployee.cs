using System.ComponentModel.DataAnnotations;

namespace employee_management.Models
{
    public class AddorEditEmployee
    {
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com)$", ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Incorrect Phone number")]
        public string Phone { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }
    }
}
