using Microsoft.AspNetCore.Mvc;
using Template.Data;
using Template.Models;

namespace Template.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
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
                TempData["Success"] = "Agency approved!";
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
                TempData["Success"] = "Agency rejected!";
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

        public IActionResult ApproveFarmer(int id)
        {
            var farmer = _context.Users.Find(id);
            if (farmer != null)
            {
                farmer.Status = "Approved";
                farmer.ApprovedBy = HttpContext.Session.GetInt32("UserId");
                _context.SaveChanges();
                TempData["Success"] = "Farmer approved!";
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