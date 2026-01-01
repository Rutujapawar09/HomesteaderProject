using Microsoft.AspNetCore.Mvc;

namespace AgriculturePortal.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Services";
            ViewData["BodyClass"] = "services-page";
            return View();
        }
    }
}