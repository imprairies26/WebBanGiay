using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class CartController : Controller
    {
        private readonly ShoesShopContext _context;

        public CartController(ShoesShopContext context)
        {
            _context = context;
        }

        private string GetSessionId()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionId")))
            {
                HttpContext.Session.SetString("SessionId", System.Guid.NewGuid().ToString());
            }
            return HttpContext.Session.GetString("SessionId")!;
        }

        private async Task<Cart> GetOrCreateCart()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var sessionId = GetSessionId();

            Cart? cart = null;

            if (!string.IsNullOrEmpty(userId))
            {
                cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                    .ThenInclude(p => p.ProductImages)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }
            else
            {
                cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                    .ThenInclude(p => p.ProductImages)
                    .FirstOrDefaultAsync(c => c.SessionId == sessionId);
            }

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    SessionId = string.IsNullOrEmpty(userId) ? sessionId : null
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await GetOrCreateCart();
            ViewData["Title"] = "Giỏ hàng";
            ViewData["ActiveNav"] = "Shop";
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int variantId, int quantity = 1)
        {
            var cart = await GetOrCreateCart();
            var variant = await _context.ProductVariants.FindAsync(variantId);

            if (variant == null || variant.StockQuantity < quantity)
            {
                return BadRequest("Sản phẩm không đủ hàng hoặc không tồn tại.");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductVariantId == variantId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductVariantId = variantId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null && quantity > 0)
            {
                var variant = await _context.ProductVariants.FindAsync(cartItem.ProductVariantId);
                if (variant != null && variant.StockQuantity >= quantity)
                {
                    cartItem.Quantity = quantity;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var cart = await GetOrCreateCart();
            int count = cart.CartItems.Sum(ci => ci.Quantity);
            return Json(new { count });
        }
    }
}
