namespace PROG6212_Part2.Models
{
    public class Lecturer : User
    {
        public string Department { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
    }
}