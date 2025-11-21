using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROG6212_Part2.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PROG6212_Part2.Controllers
{
    public class AccountController : Controller
    {
        
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [HttpGet]
        public IActionResult Login()
        {
           
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (model == null)
            {
                model = new LoginViewModel();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var hashedPassword = HashPassword(model.Password);

            
            var lecturer = LecturerController._lecturers
                .FirstOrDefault(l => l.Email.ToLower() == model.Email.ToLower() && l.Password == hashedPassword);
            if (lecturer != null)
            {
                HttpContext.Session.SetString("Role", "Lecturer");
                HttpContext.Session.SetString("Email", lecturer.Email);
                HttpContext.Session.SetString("FullName", lecturer.FullName);
                return RedirectToAction("SubmitClaim", "Lecturer");
            }

           
            var coordinator = ClaimController._coordinators
                .FirstOrDefault(c => c.Email.ToLower() == model.Email.ToLower() && c.Password == hashedPassword);
            if (coordinator != null)
            {
                HttpContext.Session.SetString("Role", "Coordinator");
                HttpContext.Session.SetString("Email", coordinator.Email);
                HttpContext.Session.SetString("FullName", coordinator.FullName);
                return RedirectToAction("CoordinatorView", "Claim");
            }

            
            var manager = ClaimController._managers
                .FirstOrDefault(m => m.Email.ToLower() == model.Email.ToLower() && m.Password == hashedPassword);
            if (manager != null)
            {
                HttpContext.Session.SetString("Role", "Manager");
                HttpContext.Session.SetString("Email", manager.Email);
                HttpContext.Session.SetString("FullName", manager.FullName);
                return RedirectToAction("ManagerView", "Claim");
            }

            
            var hr = HRController._hrs
                .FirstOrDefault(h => h.Email.ToLower() == model.Email.ToLower() && h.Password == hashedPassword);
            if (hr != null)
            {
                HttpContext.Session.SetString("Role", "HR");
                HttpContext.Session.SetString("Email", hr.Email);
                HttpContext.Session.SetString("FullName", hr.FullName);
                return RedirectToAction("Dashboard", "HR");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}