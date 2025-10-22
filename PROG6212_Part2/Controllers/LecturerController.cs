using Microsoft.AspNetCore.Mvc;
using PROG6212_Part2.Models;
using System.Security.Claims;
using Claim = PROG6212_Part2.Models.Claim;

namespace PROG6212_Part2.Controllers
{
    public class LecturerController : Controller
    {
        public static List<Lecturer> _lecturers = new()
        {
            new Lecturer
            {
                LecturerID = 1,
                FullName = "Zaara Salie",
                Email = "zaara.salie@prog.ac.za",
                Department = "Computer Science",
                PhoneNumber = "0821234567",
                Faculty = "Science & Technology"
            }
        };

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Account");

            return View(new Claim());
        }

        [HttpPost]
        public IActionResult SubmitClaim(Claim claim, IFormFile uploadedFile)
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Account");

            var email = HttpContext.Session.GetString("Email");
            var lecturer = _lecturers.FirstOrDefault(l => l.Email.ToLower() == email?.ToLower());

            if (lecturer != null)
            {
                claim.LecturerName = lecturer.FullName;
                claim.Department = lecturer.Department;
            }

            if (string.IsNullOrWhiteSpace(claim.Month) || claim.HoursWorked <= 0 || claim.Rate <= 0)
            {
                ViewBag.Error = "Please fill all required fields with valid values.";
                return View(claim);
            }

            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                ViewBag.Error = "Please upload a valid document.";
                return View(claim);
            }

            var ext = Path.GetExtension(uploadedFile.FileName).ToLower();
            if (ext != ".pdf" && ext != ".docx" && ext != ".xlsx")
            {
                ViewBag.Error = "Only .pdf, .docx, and .xlsx files are allowed.";
                return View(claim);
            }

            if (uploadedFile.Length > 5 * 1024 * 1024)
            {
                ViewBag.Error = "File size must be under 5MB.";
                return View(claim);
            }

            var fileName = Path.GetFileName(uploadedFile.FileName);
            var encryptedPath = Path.Combine("wwwroot/encrypted", fileName);
            ClaimController.SaveEncryptedFile(uploadedFile, encryptedPath);

            claim.ClaimID = ClaimController._claims.Count + 1;
            claim.DocumentName = fileName;
            claim.EncryptedPath = encryptedPath;
            claim.Status = "Submitted";

            ClaimController._claims.Add(claim);

            ViewBag.Message = "Claim submitted successfully.";
            return View(new Claim());
        }

        public IActionResult ViewClaims()
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Account");

            var email = HttpContext.Session.GetString("Email");
            var lecturer = _lecturers.FirstOrDefault(l => l.Email.ToLower() == email?.ToLower());

            var claims = ClaimController._claims
                .Where(c => c.LecturerName == lecturer?.FullName)
                .ToList();

            return View(claims);
        }
    }
}
