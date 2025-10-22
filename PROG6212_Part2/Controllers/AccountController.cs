using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROG6212_Part2.Models;
using System.Linq;


namespace PROG6212_Part2.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email)
        {
            email = email?.Trim().ToLower();

            if (LecturerController._lecturers.Any(l => l.Email.ToLower() == email))
            {
                HttpContext.Session.SetString("Role", "Lecturer");
                HttpContext.Session.SetString("Email", email);
                return RedirectToAction("SubmitClaim", "Lecturer");
            }

            if (ClaimController._coordinator.Email.ToLower() == email)
            {
                HttpContext.Session.SetString("Role", "Coordinator");
                HttpContext.Session.SetString("Email", email);
                return RedirectToAction("CoordinatorView", "Claim");
            }

            if (ClaimController._manager.Email.ToLower() == email)
            {
                HttpContext.Session.SetString("Role", "Manager");
                HttpContext.Session.SetString("Email", email);
                return RedirectToAction("ManagerView", "Claim");
            }

            ViewBag.Error = "Email not recognized.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
