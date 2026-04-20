using Microsoft.AspNetCore.Mvc;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Shop";
            ViewData["ActiveNav"] = "Shop";
            return View();
        }
    }
}
