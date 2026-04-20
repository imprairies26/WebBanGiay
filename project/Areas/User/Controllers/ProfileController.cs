using Microsoft.AspNetCore.Mvc;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Profile & Orders";
            ViewData["ActiveNav"] = "Profile";
            return View();
        }
    }
}
