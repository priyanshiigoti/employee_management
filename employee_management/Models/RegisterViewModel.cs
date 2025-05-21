using System.ComponentModel.DataAnnotations;

namespace employee_management.Models
{
    public class RegisterViewModel
    {
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|edu)$", ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
