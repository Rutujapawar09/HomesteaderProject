using Microsoft.AspNetCore.Mvc;
using Template.Data;

namespace Template.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuperAdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Dashboard
        public IActionResult Dashboard()
        {
            // Check if logged in as Super Admin
            if (HttpContext.Session.GetString("UserRole") != "SuperAdmin")
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
        // View Pending Admins
        public IActionResult PendingAdmins()
        {
            if (HttpContext.Session.GetString("UserRole") != "SuperAdmin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var admins = _context.Users
                .Where(u => u.Role == "Admin" && u.Status == "Pending")
                .ToList();

            return View(admins);
        }

        // view all users
        public IActionResult AllUsers()
        {
            if (HttpContext.Session.GetString("UserRole") != "SuperAdmin")
            {
                return RedirectToAction("Login", "Auth");
            }
            var users = _context.Users.ToList();
            return View(users);
        }
        // Approve Admin
        public IActionResult ApproveAdmin(int id)
        {
            var admin = _context.Users.Find(id);
            if (admin != null)
            {
                admin.Status = "Approved";
                admin.ApprovedBy = HttpContext.Session.GetInt32("UserId");
                _context.SaveChanges();

                TempData["Success"] = "Admin approved successfully!";
            }

            return RedirectToAction("PendingAdmins");
        }
        // Reject Admin
        public IActionResult RejectAdmin(int id)
        {
            var admin = _context.Users.Find(id);
            if (admin != null)
            {
                admin.Status = "Rejected";
                _context.SaveChanges();

                TempData["Success"] = "Admin rejected!";
            }

            return RedirectToAction("PendingAdmins");
        }
    }
}
