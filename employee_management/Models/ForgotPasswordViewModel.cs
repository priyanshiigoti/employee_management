using System.ComponentModel.DataAnnotations;

namespace employee_management.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|edu)$", ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }
    }
}
