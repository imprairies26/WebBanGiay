using Microsoft.AspNetCore.Mvc;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Manage Users";
            ViewData["ActiveNav"] = "Users";
            return View();
        }
    }
}
