using Microsoft.AspNetCore.Mvc;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Manage Products";
            ViewData["ActiveNav"] = "Products";
            return View();
        }
    }
}
