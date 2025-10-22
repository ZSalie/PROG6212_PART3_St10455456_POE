namespace PROG6212_Part2.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }
        public string LecturerName { get; set; }
        public string Department { get; set; }
        public string Month { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal Rate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string DocumentName { get; set; }
        public string EncryptedPath { get; set; }

        // ✅ Computed property
        public decimal TotalAmount => HoursWorked * Rate;
    }
}
