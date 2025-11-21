using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROG6212_Part2.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PROG6212_Part2.Controllers
{
    public class HRController : Controller
    {
        public static List<HR> _hrs = new()
        {
            new HR
            {
                ID = 1,
                FullName = "Sarah Peterson",
                Email = "hr@college.ac.za",
                Password = AccountController.HashPassword("hr123"),
                Role = "HR",
                Department = "Human Resources"
            }
        };

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "HR")
                return RedirectToAction("Login", "Account");

            var dashboardData = new HRDashboardViewModel
            {
                TotalClaims = ClaimController._claims.Count,
                SubmittedClaims = ClaimController._claims.Count(c => c.Status == "Submitted"),
                VerifiedClaims = ClaimController._claims.Count(c => c.Status == "Verified"),
                ApprovedClaims = ClaimController._claims.Count(c => c.Status == "Approved"),
                RejectedClaims = ClaimController._claims.Count(c => c.Status == "Rejected"),
                TotalLecturers = LecturerController._lecturers.Count,
                TotalCoordinators = ClaimController._coordinators.Count,
                TotalManagers = ClaimController._managers.Count,
                TotalHR = _hrs.Count
            };

            return View(dashboardData);
        }

        public IActionResult ViewAllClaims()
        {
            if (HttpContext.Session.GetString("Role") != "HR")
                return RedirectToAction("Login", "Account");

            var allClaims = ClaimController._claims
                .OrderByDescending(c => c.ClaimID)
                .ToList();

            return View(allClaims);
        }

        [HttpGet]
        public IActionResult LecturerClaimsSummary()
        {
            if (HttpContext.Session.GetString("Role") != "HR")
                return RedirectToAction("Login", "Account");

            // Group claims by lecturer and calculate summaries
            var lecturerSummaries = ClaimController._claims
                .Where(c => !string.IsNullOrEmpty(c.LecturerName))
                .GroupBy(c => new { c.LecturerName, c.Department })
                .Select(g => new LecturerSummaryViewModel
                {
                    LecturerName = g.Key.LecturerName ?? string.Empty,
                    Department = g.Key.Department ?? string.Empty,
                    TotalClaims = g.Count(),
                    SubmittedClaims = g.Count(c => c.Status == "Submitted"),
                    VerifiedClaims = g.Count(c => c.Status == "Verified"),
                    ApprovedClaims = g.Count(c => c.Status == "Approved"),
                    RejectedClaims = g.Count(c => c.Status == "Rejected"),
                    TotalAmount = g.Sum(c => c.TotalAmount),
                    AverageAmount = g.Count() > 0 ? g.Average(c => c.TotalAmount) : 0
                })
                .OrderByDescending(l => l.TotalAmount)
                .ThenBy(l => l.LecturerName)
                .ToList();

            var model = new LecturerClaimsSummaryViewModel
            {
                LecturerSummaries = lecturerSummaries,
                TotalLecturers = lecturerSummaries.Count,
                OverallTotalClaims = ClaimController._claims.Count,
                OverallTotalAmount = ClaimController._claims.Sum(c => c.TotalAmount),
                OverallSubmitted = ClaimController._claims.Count(c => c.Status == "Submitted"),
                OverallVerified = ClaimController._claims.Count(c => c.Status == "Verified"),
                OverallApproved = ClaimController._claims.Count(c => c.Status == "Approved"),
                OverallRejected = ClaimController._claims.Count(c => c.Status == "Rejected")
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ManageUsers()
        {
            if (HttpContext.Session.GetString("Role") != "HR")
                return RedirectToAction("Login", "Account");

            var userManagementModel = new UserManagementViewModel
            {
                Lecturers = LecturerController._lecturers,
                Coordinators = ClaimController._coordinators,
                Managers = ClaimController._managers,
                HRs = _hrs
            };

            return View(userManagementModel);
        }

        [HttpGet]
        public IActionResult AddUser(string userType)
        {
            if (HttpContext.Session.GetString("Role") != "HR")
                return RedirectToAction("Login", "Account");

            ViewBag.UserType = userType;

            // Create a new model to avoid null reference issues
            var model = new UserViewModel
            {
                ID = 0 // 0 indicates new user
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(string userType, UserViewModel user)
        {
            if (HttpContext.Session.GetString("Role") != "HR")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                try
                {
                    var hashedPassword = AccountController.HashPassword(user.Password ?? "default123");

                    switch (userType.ToLower())
                    {
                        case "lecturer":
                            var newLecturer = new Lecturer
                            {
                                ID = LecturerController._lecturers.Count + 1,
                                FullName = user.FullName ?? string.Empty,
                                Email = user.Email ?? string.Empty,
                                Password = hashedPassword,
                                Role = "Lecturer",
                                Department = user.Department ?? string.Empty,
                                PhoneNumber = user.PhoneNumber ?? string.Empty,
                                Faculty = user.Faculty ?? string.Empty
                            };
                            LecturerController._lecturers.Add(newLecturer);
                            TempData["Success"] = $"Lecturer {user.FullName} added successfully!";
                            break;

                        case "coordinator":
                            var newCoordinator = new Coordinator
                            {
                                ID = ClaimController._coordinators.Count + 1,
                                FullName = user.FullName ?? string.Empty,
                                Email = user.Email ?? string.Empty,
                                Password = hashedPassword,
                                Role = "Coordinator",
                                Department = user.Department ?? string.Empty
                            };
                            ClaimController._coordinators.Add(newCoordinator);
                            TempData["Success"] = $"Coordinator {user.FullName} added successfully!";
                            break;

                        case "manager":
                            var newManager = new Manager
                            {
                                ID = ClaimController._managers.Count + 1,
                                FullName = user.FullName ?? string.Empty,
                                Email = user.Email ?? string.Empty,
                                Password = hashedPassword,
                                Role = "Manager",
                                Faculty = user.Faculty ?? string.Empty
                            };
                            ClaimController._managers.Add(newManager);
                            TempData["Success"] = $"Manager {user.FullName} added successfully!";
                            break;

                        case "hr":
                            var newHR = new HR
                            {
                                ID = _hrs.Count + 1,
                                FullName = user.FullName ?? string.Empty,
                                Email = user.Email ?? string.Empty,
                                Password = hashedPassword,
                                Role = "HR",
                                Department = user.Department ?? string.Empty
                            };
                            _hrs.Add(newHR);
                            TempData["Success"] = $"HR User {user.FullName} added successfully!";
                            break;
                    }

                    return RedirectToAction("ManageUsers");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error adding user: {ex.Message}";
                }
            }

            ViewBag.UserType = userType;
            return View(user);
        }

        [HttpGet]
        public IActionResult EditUser(string userType, int id)
        {
            if (HttpContext.Session.GetString("Role") != "HR")
                return RedirectToAction("Login", "Account");

            ViewBag.UserType = userType;
            UserViewModel userModel = null;

            switch (userType.ToLower())
            {
                case "lecturer":
                    var lecturer = LecturerController._lecturers.FirstOrDefault(l => l.ID == id);
                    if (lecturer != null)
                    {
                        userModel = new UserViewModel
                        {
                            ID = lecturer.ID,
                            FullName = lecturer.FullName ?? string.Empty,
                            Email = lecturer.Email ?? string.Empty,
                            Department = lecturer.Department ?? string.Empty,
                            PhoneNumber = lecturer.PhoneNumber ?? string.Empty,
                            Faculty = lecturer.Faculty ?? string.Empty
                        };
                    }
                    break;

                case "coordinator":
                    var coordinator = ClaimController._coordinators.FirstOrDefault(c => c.ID == id);
                    if (coordinator != null)
                    {
                        userModel = new UserViewModel
                        {
                            ID = coordinator.ID,
                            FullName = coordinator.FullName ?? string.Empty,
                            Email = coordinator.Email ?? string.Empty,
                            Department = coordinator.Department ?? string.Empty
                        };
                    }
                    break;

                case "manager":
                    var manager = ClaimController._managers.FirstOrDefault(m => m.ID == id);
                    if (manager != null)
                    {
                        userModel = new UserViewModel
                        {
                            ID = manager.ID,
                            FullName = manager.FullName ?? string.Empty,
                            Email = manager.Email ?? string.Empty,
                            Faculty = manager.Faculty ?? string.Empty
                        };
                    }
                    break;

                case "hr":
                    var hr = _hrs.FirstOrDefault(h => h.ID == id);
                    if (hr != null)
                    {
                        userModel = new UserViewModel
                        {
                            ID = hr.ID,
                            FullName = hr.FullName ?? string.Empty,
                            Email = hr.Email ?? string.Empty,
                            Department = hr.Department ?? string.Empty
                        };
                    }
                    break;
            }

            if (userModel == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("ManageUsers");
            }

            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(string userType, int id, UserViewModel user)
        {
            if (HttpContext.Session.GetString("Role") != "HR")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                try
                {
                    switch (userType.ToLower())
                    {
                        case "lecturer":
                            var lecturer = LecturerController._lecturers.FirstOrDefault(l => l.ID == id);
                            if (lecturer != null)
                            {
                                lecturer.FullName = user.FullName ?? string.Empty;
                                lecturer.Email = user.Email ?? string.Empty;
                                if (!string.IsNullOrEmpty(user.Password))
                                {
                                    lecturer.Password = AccountController.HashPassword(user.Password);
                                }
                                lecturer.Department = user.Department ?? string.Empty;
                                lecturer.PhoneNumber = user.PhoneNumber ?? string.Empty;
                                lecturer.Faculty = user.Faculty ?? string.Empty;
                            }
                            break;

                        case "coordinator":
                            var coordinator = ClaimController._coordinators.FirstOrDefault(c => c.ID == id);
                            if (coordinator != null)
                            {
                                coordinator.FullName = user.FullName ?? string.Empty;
                                coordinator.Email = user.Email ?? string.Empty;
                                if (!string.IsNullOrEmpty(user.Password))
                                {
                                    coordinator.Password = AccountController.HashPassword(user.Password);
                                }
                                coordinator.Department = user.Department ?? string.Empty;
                            }
                            break;

                        case "manager":
                            var manager = ClaimController._managers.FirstOrDefault(m => m.ID == id);
                            if (manager != null)
                            {
                                manager.FullName = user.FullName ?? string.Empty;
                                manager.Email = user.Email ?? string.Empty;
                                if (!string.IsNullOrEmpty(user.Password))
                                {
                                    manager.Password = AccountController.HashPassword(user.Password);
                                }
                                manager.Faculty = user.Faculty ?? string.Empty;
                            }
                            break;

                        case "hr":
                            var hr = _hrs.FirstOrDefault(h => h.ID == id);
                            if (hr != null)
                            {
                                hr.FullName = user.FullName ?? string.Empty;
                                hr.Email = user.Email ?? string.Empty;
                                if (!string.IsNullOrEmpty(user.Password))
                                {
                                    hr.Password = AccountController.HashPassword(user.Password);
                                }
                                hr.Department = user.Department ?? string.Empty;
                            }
                            break;
                    }

                    TempData["Success"] = $"{userType} updated successfully!";
                    return RedirectToAction("ManageUsers");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error updating user: {ex.Message}";
                }
            }

            ViewBag.UserType = userType;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(string userType, int id)
        {
            if (HttpContext.Session.GetString("Role") != "HR")
                return RedirectToAction("Login", "Account");

            try
            {
                switch (userType.ToLower())
                {
                    case "lecturer":
                        var lecturer = LecturerController._lecturers.FirstOrDefault(l => l.ID == id);
                        if (lecturer != null)
                        {
                            LecturerController._lecturers.Remove(lecturer);
                            TempData["Success"] = "Lecturer deleted successfully!";
                        }
                        break;

                    case "coordinator":
                        var coordinator = ClaimController._coordinators.FirstOrDefault(c => c.ID == id);
                        if (coordinator != null)
                        {
                            ClaimController._coordinators.Remove(coordinator);
                            TempData["Success"] = "Coordinator deleted successfully!";
                        }
                        break;

                    case "manager":
                        var manager = ClaimController._managers.FirstOrDefault(m => m.ID == id);
                        if (manager != null)
                        {
                            ClaimController._managers.Remove(manager);
                            TempData["Success"] = "Manager deleted successfully!";
                        }
                        break;

                    case "hr":
                        var hr = _hrs.FirstOrDefault(h => h.ID == id);
                        if (hr != null && hr.ID != 1)
                        {
                            _hrs.Remove(hr);
                            TempData["Success"] = "HR user deleted successfully!";
                        }
                        else
                        {
                            TempData["Error"] = "Cannot delete the primary HR user.";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting user: {ex.Message}";
            }

            return RedirectToAction("ManageUsers");
        }
    }
}