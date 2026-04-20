using Microsoft.AspNetCore.Mvc;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class ProductController : Controller
    {
        public IActionResult Detail(string? id)
        {
            ViewData["Title"] = "Product Detail";
            ViewData["ActiveNav"] = "Shop";
            ViewData["ProductId"] = id;
            return View();
        }
    }
}
