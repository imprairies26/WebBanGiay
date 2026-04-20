using Microsoft.AspNetCore.Mvc;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Shopping Cart";
            ViewData["ActiveNav"] = "Shop";
            return View();
        }
    }
}
