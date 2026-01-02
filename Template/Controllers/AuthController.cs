using Microsoft.AspNetCore.Mvc;
using Template.Data;
using Template.Models;

namespace Template.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Validate Login
        [HttpPost]
        public IActionResult Login(string Email, string Password, string Role)
        {
            // Find user
            var user = _context.Users
                .FirstOrDefault(u => u.Email == Email &&
                                    u.Password == Password &&
                                    u.Role == Role);

            // Check if user exists
            if (user == null)
            {
                ViewBag.Error = "Invalid credentials or role";
                return View();
            }

            // Check if approved
            if (user.Status != "Approved")
            {
                ViewBag.Error = "Your account is not approved yet";
                return View();
            }

            // Store user info in session
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Name);

            // Redirect based on role
            return user.Role switch
            {
                "SuperAdmin" => RedirectToAction("Dashboard", "SuperAdmin"),
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Farmer" => RedirectToAction("Dashboard", "Farmer"),
                "Student" => RedirectToAction("Dashboard", "Student"),
                "Agency" => RedirectToAction("Dashboard", "Agency"),
                _ => RedirectToAction("Login")
            };
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        // POST: Save Registration Data
        [HttpPost]
        public IActionResult Register(User user)
        {
            // Set default status as Pending
            user.Status = "Pending";

            // Save to database
            _context.Users.Add(user);
            _context.SaveChanges();

            // Show success message
            TempData["Success"] = "Registration successful! Wait for admin approval.";

            return RedirectToAction("Login");
        }
    }
}
