using Microsoft.AspNetCore.Mvc;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            ViewData["Title"] = "Authentication";
            ViewData["ActiveNav"] = "Login";
            return View();
        }

        public IActionResult Register()
        {
            ViewData["Title"] = "Register";
            return View("Login");
        }
    }
}
