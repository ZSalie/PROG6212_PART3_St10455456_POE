namespace PROG6212_Part2.Models
{
    public class ManagerDashboardViewModel
    {
        public string ManagerName { get; set; }
        public string Faculty { get; set; }
        public int TotalClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int VerifiedClaims { get; set; }
        public int SubmittedClaims { get; set; }
        public int RejectedClaims { get; set; }
    }
}