using System.ComponentModel.DataAnnotations;

namespace PROG6212_Part2.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }

       
        public string LecturerName { get; set; }
        public string Department { get; set; }

        [Required(ErrorMessage = "Month is required")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Course code is required")]
        [StringLength(20, ErrorMessage = "Course code must not exceed 20 characters")]
        public string CourseCode { get; set; }

        [Required(ErrorMessage = "Course title is required")]
        [StringLength(100, ErrorMessage = "Course title must not exceed 100 characters")]
        public string CourseTitle { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0.5, 200, ErrorMessage = "Hours must be between 0.5 and 200")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Rate is required")]
        [Range(1, 1000, ErrorMessage = "Rate must be between R1 and R1000")]
        public decimal Rate { get; set; }

        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
        public string Notes { get; set; }

       
        public string Status { get; set; }
        public string DocumentName { get; set; }
        public string EncryptedPath { get; set; }

       
        public decimal TotalAmount => HoursWorked * Rate;
    }
}