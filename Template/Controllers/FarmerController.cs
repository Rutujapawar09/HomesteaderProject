using Microsoft.AspNetCore.Mvc;

namespace Template.Controllers
{
    public class FarmerController : Controller
    {
        public IActionResult Dashboard()
        {
            // Optional security check
            if (HttpContext.Session.GetString("UserRole") != "Farmer")
                return RedirectToAction("Login", "Auth");

            return View();
        }
    }
}
