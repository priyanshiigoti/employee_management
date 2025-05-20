using System.ComponentModel.DataAnnotations;

namespace employee_management.Models
{
    public class AddorEditDepartment
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
