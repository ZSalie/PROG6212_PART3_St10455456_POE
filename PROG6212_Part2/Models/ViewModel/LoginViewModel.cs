using System.ComponentModel.DataAnnotations;

namespace PROG6212_Part2.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class HRDashboardViewModel
    {
        public int TotalClaims { get; set; }
        public int SubmittedClaims { get; set; }
        public int VerifiedClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int RejectedClaims { get; set; }
        public int TotalLecturers { get; set; }
        public int TotalCoordinators { get; set; }
        public int TotalManagers { get; set; }
        public int TotalHR { get; set; }
    }

    public class UserManagementViewModel
    {
        public List<Lecturer> Lecturers { get; set; } = new List<Lecturer>();
        public List<Coordinator> Coordinators { get; set; } = new List<Coordinator>();
        public List<Manager> Managers { get; set; } = new List<Manager>();
        public List<HR> HRs { get; set; } = new List<HR>();
    }

    public class UserViewModel
    {
        public int ID { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
    }

    public class LecturerSummaryViewModel
    {
        public string LecturerName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int TotalClaims { get; set; }
        public int SubmittedClaims { get; set; }
        public int VerifiedClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int RejectedClaims { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
        public decimal ApprovalRate => TotalClaims > 0 ? (decimal)ApprovedClaims / TotalClaims * 100 : 0;
        public decimal RejectionRate => TotalClaims > 0 ? (decimal)RejectedClaims / TotalClaims * 100 : 0;
    }
    
        public class LecturerClaimsSummaryViewModel
        {
            public List<LecturerSummaryViewModel> LecturerSummaries { get; set; } = new List<LecturerSummaryViewModel>();
            public int TotalLecturers { get; set; }
            public int OverallTotalClaims { get; set; }
            public decimal OverallTotalAmount { get; set; }
            public int OverallSubmitted { get; set; }
            public int OverallVerified { get; set; }
            public int OverallApproved { get; set; }
            public int OverallRejected { get; set; }

            // Computed overall percentages
            public decimal OverallApprovalRate => OverallTotalClaims > 0 ? (decimal)OverallApproved / OverallTotalClaims * 100 : 0;
            public decimal OverallRejectionRate => OverallTotalClaims > 0 ? (decimal)OverallRejected / OverallTotalClaims * 100 : 0;
        }
   

    public class SummaryReportViewModel
    {
        public List<LecturerSummaryViewModel> LecturerSummaries { get; set; } = new List<LecturerSummaryViewModel>();
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
        public string ReportPeriod { get; set; } = "All Time";
        public decimal GrandTotalHours { get; set; }
        public decimal GrandTotalAmount { get; set; }
        public int GrandTotalClaims { get; set; }
    }

  
}