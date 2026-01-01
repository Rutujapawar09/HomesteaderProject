using Microsoft.AspNetCore.Mvc;

namespace AgriculturePortal.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "About Us";
            ViewData["BodyClass"] = "about-page";
            return View();
        }
    }
}