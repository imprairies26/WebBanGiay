using Microsoft.AspNetCore.Mvc;

namespace project.Areas.POS.Controllers
{
    [Area("POS")]
    public class SaleController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "POS Terminal";
            return View();
        }
    }
}
