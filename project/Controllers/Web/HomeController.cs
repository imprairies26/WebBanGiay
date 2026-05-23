using Microsoft.AspNetCore.Mvc;

namespace project.Controllers.Web
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        [Route("/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;
            if (statusCode == 404) return View("NotFound");
            return View("Error");
        }
    }
}
