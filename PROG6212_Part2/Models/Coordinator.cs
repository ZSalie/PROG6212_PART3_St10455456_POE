using System.ComponentModel.DataAnnotations;

namespace PROG6212_Part2.Models
{
    public class Coordinator
    {
        public int CoordinatorID { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;
    }
}
