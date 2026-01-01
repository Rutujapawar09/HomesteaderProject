using Microsoft.AspNetCore.Mvc;

namespace Template.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
