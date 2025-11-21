using System.ComponentModel.DataAnnotations;

namespace PROG6212_Part2.Models
{
    public abstract class User
    {
        public int ID { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}