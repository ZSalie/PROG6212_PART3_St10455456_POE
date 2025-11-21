using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG6212_Part2.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PROG6212_Part2.Controllers
{
    public class ClaimController : Controller
    {
        public static List<Models.Claim> _claims = new();

        public static List<Coordinator> _coordinators = new()
        {
            new Coordinator
            {
                ID = 1,
                FullName = "Thabo Mokoena",
                Email = "thabo.mokoena@college.ac.za",
                Password = AccountController.HashPassword("coordinator123"),
                Role = "Coordinator",
                Department = "Computer Science"
            }
        };

        public static List<Manager> _managers = new()
        {
            new Manager
            {
                ID = 1,
                FullName = "Naledi Jacobs",
                Email = "naledi.jacobs@college.ac.za",
                Password = AccountController.HashPassword("manager123"),
                Role = "Manager",
                Faculty = "Science & Technology"
            }
        };

        

        [HttpGet]
        public IActionResult CoordinatorView()
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Account");

            var submittedClaims = _claims.Where(c => c.Status == "Submitted").ToList();
            return View(submittedClaims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Verify(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Account");

            var claim = _claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Verified";
                TempData["Success"] = "Claim verified successfully!";
            }
            return RedirectToAction("CoordinatorView");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Account");

            var claim = _claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Rejected";
                TempData["Success"] = "Claim rejected successfully!";
            }
            return RedirectToAction("CoordinatorView");
        }

        

        [HttpGet]
        public IActionResult ManagerView()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Account");

            var verifiedClaims = _claims.Where(c => c.Status == "Verified").ToList();
            return View(verifiedClaims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Account");

            var claim = _claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Approved";
                TempData["Success"] = "Claim approved successfully!";
            }
            return RedirectToAction("ManagerView");
        }

        [HttpGet]
        public IActionResult ManagerDashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Account");

            var email = HttpContext.Session.GetString("Email");
            var manager = _managers.FirstOrDefault(m => m.Email.ToLower() == email?.ToLower());

            var model = new
            {
                ManagerName = manager?.FullName,
                Faculty = manager?.Faculty,
                TotalClaims = _claims.Count,
                ApprovedClaims = _claims.Count(c => c.Status == "Approved"),
                VerifiedClaims = _claims.Count(c => c.Status == "Verified"),
                SubmittedClaims = _claims.Count(c => c.Status == "Submitted"),
                RejectedClaims = _claims.Count(c => c.Status == "Rejected")
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ManagerAllClaims()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Account");

            var allClaims = _claims.OrderByDescending(c => c.ClaimID).ToList();
            return View(allClaims);
        }

        

        public static void SaveEncryptedFile(IFormFile file, string path)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            byte[] data = ms.ToArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= 0x5A;
            }
            System.IO.File.WriteAllBytes(path, data);
        }

        public IActionResult Download(int id)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim == null || string.IsNullOrEmpty(claim.EncryptedPath))
            {
                return NotFound();
            }

            byte[] data = System.IO.File.ReadAllBytes(claim.EncryptedPath);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= 0x5A;
            }

            return new FileContentResult(data, "application/octet-stream")
            {
                FileDownloadName = claim.DocumentName
            };
        }

        [HttpGet]
        public IActionResult ViewAllClaims()
        {
           
            var userRole = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(userRole))
                return RedirectToAction("Login", "Account");

            var allClaims = _claims.OrderByDescending(c => c.ClaimID).ToList();

            
            if (userRole == "HR")
            {
                return View("HRAllClaims", allClaims);
            }
            else if (userRole == "Manager")
            {
                return View("ManagerAllClaims", allClaims);
            }
            else
            {
               
                return View(allClaims);
            }
        }
    }
}