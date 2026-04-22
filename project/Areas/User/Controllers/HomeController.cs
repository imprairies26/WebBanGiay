using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ShoesShopContext _context;

        public HomeController(ShoesShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? search, 
            int[]? categories, 
            string[]? sizes, 
            string[]? colors, 
            string? sortBy, 
            int page = 1)
        {
            ViewData["Title"] = "Shop";
            ViewData["ActiveNav"] = "Shop";

            int pageSize = 12;
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Where(p => p.IsActive)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));
            }

            // Categories
            if (categories != null && categories.Length > 0)
            {
                query = query.Where(p => categories.Contains(p.CategoryId));
            }

            // Sizes & Colors (from ProductVariants)
            if ((sizes != null && sizes.Length > 0) || (colors != null && colors.Length > 0))
            {
                query = query.Where(p => p.ProductVariants.Any(v => 
                    (sizes == null || sizes.Length == 0 || sizes.Contains(v.Size)) &&
                    (colors == null || colors.Length == 0 || colors.Contains(v.Color))
                ));
            }

            // Sorting
            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.SalePrice ?? p.BasePrice);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.SalePrice ?? p.BasePrice);
                    break;
                case "newest":
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt);
                    break;
            }

            int totalItems = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ProductHomeViewModel
            {
                Products = products,
                Categories = await _context.Categories.ToListAsync(),
                SelectedCategories = categories?.Select(c => c.ToString()).ToList() ?? new List<string>(),
                SelectedSizes = sizes?.ToList() ?? new List<string>(),
                SelectedColors = colors?.ToList() ?? new List<string>(),
                SearchQuery = search,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize),
                TotalItems = totalItems
            };

            // Fetch distinct sizes and colors for filter UI
            ViewBag.AvailableSizes = await _context.ProductVariants.Select(v => v.Size).Distinct().ToListAsync();
            ViewBag.AvailableColors = await _context.ProductVariants.Select(v => v.Color).Distinct().ToListAsync();

            return View(viewModel);
        }
    }
}
