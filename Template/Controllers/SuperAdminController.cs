using Microsoft.AspNetCore.Mvc;
using Template.Data;
using Template.Services;

namespace Template.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public SuperAdminController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Dashboard with Statistics
        public IActionResult Dashboard()
        {
            // Check if logged in as Super Admin
            if (HttpContext.Session.GetString("UserRole") != "SuperAdmin")
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get statistics
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.PendingAdmins = _context.Users.Count(u => u.Role == "Admin" && u.Status == "Pending");
            ViewBag.ApprovedUsers = _context.Users.Count(u => u.Status == "Approved");
            ViewBag.TotalFarmers = _context.Users.Count(u => u.Role == "Farmer");

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
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            return View(admins);
        }

        // View All Users
        public IActionResult AllUsers()
        {
            if (HttpContext.Session.GetString("UserRole") != "SuperAdmin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var users = _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            return View(users);
        }

        // Approve Admin (WITH EMAIL NOTIFICATION)
        public IActionResult ApproveAdmin(int id)
        {
            var admin = _context.Users.Find(id);
            if (admin != null)
            {
                admin.Status = "Approved";
                admin.ApprovedBy = HttpContext.Session.GetInt32("UserId");
                _context.SaveChanges();

                // Send approval email
                try
                {
                    string subject = "✅ Your Admin Account is Approved!";
                    string body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='background-color: #f8f9fa; padding: 20px;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto;'>
                                    <h2 style='color: #28a745;'>🎉 Congratulations {admin.Name}!</h2>
                                    <p style='font-size: 16px;'>Your admin account has been <strong>approved</strong> by the Super Admin.</p>
                                    
                                    <div style='background-color: #e9ecef; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                        <h3 style='margin-top: 0;'>Login Details:</h3>
                                        <p><strong>Email:</strong> {admin.Email}</p>
                                        <p><strong>Role:</strong> Admin</p>
                                        <p><strong>Status:</strong> ✅ Approved</p>
                                    </div>

                                    <p>You can now login to the Homesteader system and start managing the platform.</p>
                                    
                                    <a href='http://localhost:5000/Auth/Login' 
                                       style='display: inline-block; background-color: #28a745; color: white; 
                                              padding: 12px 30px; text-decoration: none; border-radius: 5px; 
                                              margin-top: 20px;'>
                                        Login Now
                                    </a>

                                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #dee2e6;'>
                                    <p style='color: #6c757d; font-size: 14px;'>
                                        This is an automated email from Homesteader System.<br>
                                        Please do not reply to this email.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    _emailService.SendEmail(admin.Email, subject, body);
                    TempData["Success"] = "Admin approved successfully and email sent!";
                }
                catch
                {
                    TempData["Success"] = "Admin approved successfully (but email failed to send)";
                }
            }

            return RedirectToAction("PendingAdmins");
        }

        // Reject Admin (WITH EMAIL NOTIFICATION)
        public IActionResult RejectAdmin(int id)
        {
            var admin = _context.Users.Find(id);
            if (admin != null)
            {
                admin.Status = "Rejected";
                _context.SaveChanges();

                // Send rejection email
                try
                {
                    string subject = "❌ Admin Account Registration Update";
                    string body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='background-color: #f8f9fa; padding: 20px;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto;'>
                                    <h2 style='color: #dc3545;'>Registration Update</h2>
                                    <p>Dear {admin.Name},</p>
                                    <p style='font-size: 16px;'>We regret to inform you that your admin registration has been <strong>rejected</strong>.</p>
                                    
                                    <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #dc3545;'>
                                        <p style='margin: 0;'><strong>Status:</strong> ❌ Rejected</p>
                                    </div>

                                    <p>If you believe this is a mistake, please contact the Super Admin for more information.</p>

                                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #dee2e6;'>
                                    <p style='color: #6c757d; font-size: 14px;'>
                                        This is an automated email from Homesteader System.<br>
                                        Please do not reply to this email.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    _emailService.SendEmail(admin.Email, subject, body);
                    TempData["Success"] = "Admin rejected and email sent!";
                }
                catch
                {
                    TempData["Success"] = "Admin rejected (but email failed to send)";
                }
            }

            return RedirectToAction("PendingAdmins");
        }

        // Delete User (Optional - for cleaning up test data)
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            // Prevent deleting Super Admin
            if (user != null && user.Role != "SuperAdmin")
            {
                _context.Users.Remove(user);
                _context.SaveChanges();

                TempData["Success"] = "User deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Cannot delete Super Admin!";
            }

            return RedirectToAction("AllUsers");
        }
    }
}