using Microsoft.AspNetCore.Mvc;
using PROG6212_Part2.Models;
using System.Security.Claims;
using Claim = PROG6212_Part2.Models.Claim;
using System.Security.Cryptography;
using System.Text;

namespace PROG6212_Part2.Controllers
{
    public class LecturerController : Controller
    {
        public static List<Lecturer> _lecturers = new()
        {
            new Lecturer
            {
                ID = 1,
                FullName = "Zaara Salie",
                Email = "zaara.salie@prog.ac.za",
                Password = AccountController.HashPassword("lecturer123"),
                Role = "Lecturer",
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

            var email = HttpContext.Session.GetString("Email");
            var lecturer = _lecturers.FirstOrDefault(l => l.Email.ToLower() == email?.ToLower());

            var claim = new Claim();
            if (lecturer != null)
            {
                claim.LecturerName = lecturer.FullName;
                claim.Department = lecturer.Department;
            }

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile uploadedFile)
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Account");

            var email = HttpContext.Session.GetString("Email");
            var lecturer = _lecturers.FirstOrDefault(l => l.Email.ToLower() == email?.ToLower());

            // Remove validation errors for system-set fields
            ModelState.Remove("LecturerName");
            ModelState.Remove("Department");
            ModelState.Remove("Status");
            ModelState.Remove("DocumentName");
            ModelState.Remove("EncryptedPath");

            // Set system fields from lecturer data
            if (lecturer != null)
            {
                claim.LecturerName = lecturer.FullName;
                claim.Department = lecturer.Department;
            }

            // FILE VALIDATION FIRST - before ModelState
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                ModelState.AddModelError("uploadedFile", "Please upload a valid document.");
                ViewBag.Error = "Please upload a valid document.";
                return View(claim);
            }

            var ext = Path.GetExtension(uploadedFile.FileName).ToLower();
            if (ext != ".pdf" && ext != ".docx" && ext != ".xlsx")
            {
                ModelState.AddModelError("uploadedFile", "Only .pdf, .docx, and .xlsx files are allowed.");
                ViewBag.Error = "Only .pdf, .docx, and .xlsx files are allowed.";
                return View(claim);
            }

            if (uploadedFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("uploadedFile", "File size must be under 5MB.");
                ViewBag.Error = "File size must be under 5MB.";
                return View(claim);
            }

            // Now check other model validations
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please correct the validation errors below.";
                return View(claim);
            }

            // Business rule validation
            if (claim.HoursWorked > 160)
            {
                ModelState.AddModelError("HoursWorked", "Hours exceed part-time limit (160 hours). Please verify.");
                ViewBag.Error = "Hours exceed part-time limit (160 hours). Please verify.";
                return View(claim);
            }

            if (claim.TotalAmount > 80000)
            {
                ModelState.AddModelError("", "Monthly earnings exceed R80,000 limit. Please verify.");
                ViewBag.Error = "Monthly earnings exceed R80,000 limit. Please verify.";
                return View(claim);
            }

            try
            {
                // Create encrypted directory if it doesn't exist
                var encryptedDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "encrypted");
                if (!Directory.Exists(encryptedDir))
                {
                    Directory.CreateDirectory(encryptedDir);
                }

                // Use GUID to prevent filename conflicts
                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadedFile.FileName);
                var encryptedPath = Path.Combine(encryptedDir, fileName);

                // Save file
                using (var stream = new FileStream(encryptedPath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                // Set system fields
                claim.ClaimID = ClaimController._claims.Count + 1;
                claim.DocumentName = uploadedFile.FileName;
                claim.EncryptedPath = encryptedPath;
                claim.Status = "Submitted";

                ClaimController._claims.Add(claim);

                TempData["SuccessMessage"] = "Claim submitted successfully! Your claim is now under review.";

                // Return a new model to clear the form but keep lecturer info
                return View(new Claim
                {
                    LecturerName = lecturer?.FullName,
                    Department = lecturer?.Department
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("uploadedFile", $"An error occurred while uploading the file: {ex.Message}");
                ViewBag.Error = $"An error occurred while submitting your claim: {ex.Message}";
                return View(claim);
            }
        }

        public IActionResult ViewClaims()
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Account");

            var email = HttpContext.Session.GetString("Email");
            var lecturer = _lecturers.FirstOrDefault(l => l.Email.ToLower() == email?.ToLower());

            var claims = ClaimController._claims
                .Where(c => c.LecturerName == lecturer?.FullName)
                .OrderByDescending(c => c.ClaimID)
                .ToList();

            return View(claims);
        }
    }
}