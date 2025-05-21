using System.ComponentModel.DataAnnotations;

namespace employee_management.Models
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|edu)$", ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
