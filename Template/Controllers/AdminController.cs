using Microsoft.AspNetCore.Mvc;
using Template.Data;
using Template.Models;
using Template.Services;

namespace Template.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        // ⭐ UPDATED CONSTRUCTOR - Added EmailService
        public AdminController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Check if admin is logged in
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // Dashboard
        public IActionResult Dashboard()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            // Statistics
            ViewBag.TotalFarmers = _context.Users.Count(u => u.Role == "Farmer");
            ViewBag.TotalStudents = _context.Users.Count(u => u.Role == "Student");
            ViewBag.TotalAgencies = _context.Users.Count(u => u.Role == "Agency");
            ViewBag.TotalCrops = _context.Crops.Count();
            ViewBag.TotalFeedback = _context.Feedbacks.Count();
            ViewBag.PendingFarmers = _context.Users.Count(u => u.Role == "Farmer" && u.Status == "Pending");

            return View();
        }

        // ========== MANAGE AGENCIES ==========
        public IActionResult ManageAgencies()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            var agencies = _context.Users
                .Where(u => u.Role == "Agency")
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            return View(agencies);
        }

        public IActionResult ApproveAgency(int id)
        {
            var agency = _context.Users.Find(id);
            if (agency != null)
            {
                agency.Status = "Approved";
                agency.ApprovedBy = HttpContext.Session.GetInt32("UserId");
                _context.SaveChanges();

                // ⭐ Send approval email to Agency
                try
                {
                    string subject = "✅ Your Agency Account is Approved!";
                    string body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='background-color: #f8f9fa; padding: 20px;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto;'>
                                    <h2 style='color: #28a745;'>🎉 Congratulations {agency.Name}!</h2>
                                    <p style='font-size: 16px;'>Your agency account has been <strong>approved</strong> by the Admin.</p>
                                    
                                    <div style='background-color: #e9ecef; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                        <h3 style='margin-top: 0;'>Login Details:</h3>
                                        <p><strong>Email:</strong> {agency.Email}</p>
                                        <p><strong>Role:</strong> Agency</p>
                                        <p><strong>Status:</strong> ✅ Approved</p>
                                    </div>

                                    <p>You can now login to the Homesteader system.</p>

                                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #dee2e6;'>
                                    <p style='color: #6c757d; font-size: 14px;'>
                                        This is an automated email from Homesteader System.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    _emailService.SendEmail(agency.Email, subject, body);
                    TempData["Success"] = "Agency approved and email sent!";
                }
                catch
                {
                    TempData["Success"] = "Agency approved (email failed)";
                }
            }
            return RedirectToAction("ManageAgencies");
        }

        public IActionResult RejectAgency(int id)
        {
            var agency = _context.Users.Find(id);
            if (agency != null)
            {
                agency.Status = "Rejected";
                _context.SaveChanges();

                // ⭐ Send rejection email to Agency
                try
                {
                    string subject = "❌ Agency Account Registration Update";
                    string body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='background-color: #f8f9fa; padding: 20px;'>
                        <div style='background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                            <div style='text-align: center; margin-bottom: 20px;'>
                                <h2 style='color: #dc3545; margin: 0;'>⚠️ Registration Status Update</h2>
                            </div>

                            <p style='font-size: 16px;'>Dear <strong>{agency.Name}</strong>,</p>
                            
                            <p style='font-size: 15px; line-height: 1.6;'>
                                Thank you for your interest in registering as an <strong>Agro Agency</strong> with the Homesteader System.
                            </p>

                            <div style='background-color: #f8d7da; padding: 20px; border-radius: 5px; margin: 25px 0; border-left: 5px solid #dc3545;'>
                                <h3 style='margin-top: 0; color: #721c24;'>Registration Status</h3>
                                <p style='margin: 5px 0; font-size: 16px;'><strong>Status:</strong> <span style='color: #dc3545; font-weight: bold;'>❌ Rejected</span></p>
                                <p style='margin: 5px 0;'><strong>Email:</strong> {agency.Email}</p>
                                <p style='margin: 5px 0;'><strong>Role:</strong> Agro Agency</p>
                                <p style='margin: 5px 0;'><strong>Date:</strong> {DateTime.Now:dd MMMM yyyy}</p>
                            </div>

                            <div style='background-color: #fff3cd; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 5px solid #ffc107;'>
                                <p style='margin: 0; font-size: 14px;'>
                                    <strong>⚠️ Note:</strong> Unfortunately, your agency registration has been rejected by our admin team. 
                                    This could be due to incomplete information or verification issues.
                                </p>
                            </div>

                            <div style='margin: 25px 0;'>
                                <h4 style='color: #495057;'>What you can do:</h4>
                                <ul style='line-height: 1.8; color: #6c757d;'>
                                    <li>Contact the admin team for more details about the rejection</li>
                                    <li>Review your registration information for accuracy</li>
                                    <li>Re-apply with correct and complete details if needed</li>
                                    <li>Ensure all required documents are properly submitted</li>
                                </ul>
                            </div>

                            <div style='background-color: #e7f3ff; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                <p style='margin: 0; font-size: 14px; color: #004085;'>
                                    <strong>📞 Need Help?</strong><br>
                                    If you believe this is a mistake or need clarification, please contact our support team.
                                </p>
                            </div>

                            <hr style='margin: 30px 0; border: none; border-top: 1px solid #dee2e6;'>
                            
                            <p style='color: #6c757d; font-size: 13px; text-align: center; margin: 0;'>
                                This is an automated email from <strong>Homesteader System</strong>.<br>
                                Please do not reply directly to this email.
                            </p>
                            
                            <p style='color: #adb5bd; font-size: 12px; text-align: center; margin-top: 10px;'>
                                © {DateTime.Now.Year} Homesteader. All rights reserved.
                            </p>
                        </div>
                    </div>
                </body>
                </html>
            ";

                    _emailService.SendEmail(agency.Email, subject, body);
                    TempData["Success"] = "Agency rejected and notification email sent!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email Error: {ex.Message}");
                    TempData["Success"] = "Agency rejected (email notification failed)";
                }
            }

            return RedirectToAction("ManageAgencies");
        }


        // ========== MANAGE FARMERS ==========
        public IActionResult ManageFarmers()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            var farmers = _context.Users
                .Where(u => u.Role == "Farmer")
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            return View(farmers);
        }

        // ⭐ UPDATED: ApproveFarmer WITH EMAIL
        public IActionResult ApproveFarmer(int id)
        {
            var farmer = _context.Users.Find(id);
            if (farmer != null)
            {
                farmer.Status = "Approved";
                farmer.ApprovedBy = HttpContext.Session.GetInt32("UserId");
                _context.SaveChanges();

                // ⭐ Send approval email to Farmer
                try
                {
                    string subject = "✅ Your Farmer Account is Approved!";
                    string body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='background-color: #f8f9fa; padding: 20px;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto;'>
                                    <h2 style='color: #28a745;'>🎉 Congratulations {farmer.Name}!</h2>
                                    <p style='font-size: 16px;'>Your farmer account has been <strong>approved</strong> by the Admin.</p>
                                    
                                    <div style='background-color: #e9ecef; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                        <h3 style='margin-top: 0;'>✅ Login Details:</h3>
                                        <p><strong>Email:</strong> {farmer.Email}</p>
                                        <p><strong>Role:</strong> Farmer</p>
                                        <p><strong>Status:</strong> ✅ Approved</p>
                                    </div>

                                    <p>You can now login to the Homesteader system and access:</p>
                                    <ul>
                                        <li>🌾 View Crops Information</li>
                                        <li>💰 Market Prices</li>
                                        <li>📊 Soil Reports</li>
                                        <li>📚 Request Training</li>
                                        <li>💬 Submit Feedback</li>
                                    </ul>

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

                    _emailService.SendEmail(farmer.Email, subject, body);
                    TempData["Success"] = "Farmer approved and email sent successfully!";
                }
                catch
                {
                    TempData["Success"] = "Farmer approved (but email failed to send)";
                }
            }

            return RedirectToAction("ManageFarmers");
        }

        // ⭐ BONUS: Reject Farmer with Email
        public IActionResult RejectFarmer(int id)
        {
            var farmer = _context.Users.Find(id);
            if (farmer != null)
            {
                farmer.Status = "Rejected";
                _context.SaveChanges();

                // Send rejection email
                try
                {
                    string subject = "❌ Farmer Account Registration Update";
                    string body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='background-color: #f8f9fa; padding: 20px;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto;'>
                                    <h2 style='color: #dc3545;'>Registration Update</h2>
                                    <p>Dear {farmer.Name},</p>
                                    <p style='font-size: 16px;'>We regret to inform you that your farmer registration has been <strong>rejected</strong>.</p>
                                    
                                    <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #dc3545;'>
                                        <p style='margin: 0;'><strong>Status:</strong> ❌ Rejected</p>
                                    </div>

                                    <p>If you believe this is a mistake, please contact the Admin for more information.</p>

                                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #dee2e6;'>
                                    <p style='color: #6c757d; font-size: 14px;'>
                                        This is an automated email from Homesteader System.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    _emailService.SendEmail(farmer.Email, subject, body);
                    TempData["Success"] = "Farmer rejected and email sent!";
                }
                catch
                {
                    TempData["Success"] = "Farmer rejected!";
                }
            }

            return RedirectToAction("ManageFarmers");
        }

        // ========== MANAGE CROPS ==========
        public IActionResult ManageCrops()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            var crops = _context.Crops
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return View(crops);
        }

        [HttpGet]
        public IActionResult AddCrop()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public IActionResult AddCrop(Crop crop)
        {
            crop.CreatedBy = HttpContext.Session.GetInt32("UserId") ?? 0;
            crop.CreatedAt = DateTime.Now;

            _context.Crops.Add(crop);
            _context.SaveChanges();

            TempData["Success"] = "Crop added successfully!";
            return RedirectToAction("ManageCrops");
        }

        public IActionResult DeleteCrop(int id)
        {
            var crop = _context.Crops.Find(id);
            if (crop != null)
            {
                _context.Crops.Remove(crop);
                _context.SaveChanges();
                TempData["Success"] = "Crop deleted!";
            }
            return RedirectToAction("ManageCrops");
        }

        // ========== MARKET PRICES ==========
        public IActionResult ManagePrices()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            var prices = _context.MarketPrices
                .OrderByDescending(p => p.Date)
                .ToList();

            return View(prices);
        }

        [HttpGet]
        public IActionResult AddPrice()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public IActionResult AddPrice(MarketPrice price)
        {
            price.AddedBy = HttpContext.Session.GetInt32("UserId") ?? 0;
            price.Date = DateTime.Now;

            _context.MarketPrices.Add(price);
            _context.SaveChanges();

            TempData["Success"] = "Price added successfully!";
            return RedirectToAction("ManagePrices");
        }

        public IActionResult DeletePrice(int id)
        {
            var price = _context.MarketPrices.Find(id);
            if (price != null)
            {
                _context.MarketPrices.Remove(price);
                _context.SaveChanges();
                TempData["Success"] = "Price deleted!";
            }
            return RedirectToAction("ManagePrices");
        }

        // ========== SOIL REPORTS ==========
        public IActionResult ManageSoilReports()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            var reports = _context.SoilReports
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            return View(reports);
        }

        [HttpGet]
        public IActionResult AddSoilReport()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public IActionResult AddSoilReport(SoilReport report)
        {
            report.AddedBy = HttpContext.Session.GetInt32("UserId") ?? 0;
            report.CreatedAt = DateTime.Now;

            _context.SoilReports.Add(report);
            _context.SaveChanges();

            TempData["Success"] = "Soil report added successfully!";
            return RedirectToAction("ManageSoilReports");
        }

        // ========== VIEW FEEDBACK ==========
        public IActionResult ViewFeedback()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            var feedbacks = _context.Feedbacks
                .OrderByDescending(f => f.CreatedAt)
                .ToList();

            return View(feedbacks);
        }

        public IActionResult MarkFeedbackRead(int id)
        {
            var feedback = _context.Feedbacks.Find(id);
            if (feedback != null)
            {
                feedback.Status = "Read";
                _context.SaveChanges();
            }
            return RedirectToAction("ViewFeedback");
        }
    }
}