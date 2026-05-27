using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System.Threading.Tasks;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class ProductController : Controller
    {
        private readonly ShoesShopContext _context;

        public ProductController(ShoesShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["Title"] = product.Name;
            ViewData["ActiveNav"] = "Shop";

            return View(product);
        }
    }
}
