using Microsoft.AspNetCore.Mvc;

namespace AgriculturePortal.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Contact";
            ViewData["BodyClass"] = "contact-page";
            return View();
        }

        [HttpPost]
        public IActionResult Submit(ContactFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle form submission
                TempData["Message"] = "Your message has been sent. Thank you!";
                return RedirectToAction("Index");
            }
            return View("Index", model);
        }
    }

    public class ContactFormModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}