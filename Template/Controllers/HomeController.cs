using Microsoft.AspNetCore.Mvc;

namespace AgriculturePortal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            ViewData["BodyClass"] = "index-page";
            return View();
        }

        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            // Handle newsletter subscription
            TempData["Message"] = "Thank you for subscribing!";
            return RedirectToAction("Index");
        }
    }
}