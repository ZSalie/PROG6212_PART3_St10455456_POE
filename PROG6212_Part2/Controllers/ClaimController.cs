using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG6212_Part2.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace PROG6212_Part2.Controllers
{
    public class ClaimController : Controller
    {
        public static List<Models.Claim> _claims = new();

        public static Coordinator _coordinator = new Coordinator
        {
            CoordinatorID = 1,
            FullName = "Thabo Mokoena",
            Email = "thabo.mokoena@college.ac.za",
            Department = "Computer Science"
        };

        public static Manager _manager = new Manager
        {
            ManagerID = 1,
            FullName = "Naledi Jacobs",
            Email = "naledi.jacobs@college.ac.za",
            Faculty = "Science & Technology"
        };

        public IActionResult CoordinatorView()
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Account");

            var submittedClaims = _claims.Where(c => c.Status == "Submitted").ToList();
            return View(submittedClaims);
        }

        public IActionResult ManagerView()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Account");

            var verifiedClaims = _claims.Where(c => c.Status == "Verified").ToList();
            return View(verifiedClaims);
        }

        [HttpPost]
        public IActionResult Verify(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Account");

            var claim = _claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Verified";
            }
            return RedirectToAction("CoordinatorView");
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Account");

            var claim = _claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Approved";
            }
            return RedirectToAction("ManagerView");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Account");

            var claim = _claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Rejected";
            }
            return RedirectToAction("CoordinatorView");
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
    }
}
