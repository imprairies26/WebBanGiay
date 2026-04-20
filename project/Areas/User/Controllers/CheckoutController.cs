using Microsoft.AspNetCore.Mvc;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Checkout";
            return View();
        }
    }
}
