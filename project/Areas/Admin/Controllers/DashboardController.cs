using Microsoft.AspNetCore.Mvc;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["ActiveNav"] = "Dashboard";
            return View();
        }
    }
}
