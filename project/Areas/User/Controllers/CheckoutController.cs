using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class CheckoutController : Controller
    {
        private readonly ShoesShopContext _context;
        private readonly project.Services.IEmailService _emailService;

        public CheckoutController(ShoesShopContext context, project.Services.IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        private async Task<Cart?> GetCart()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var sessionId = HttpContext.Session.GetString("SessionId");

            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(sessionId))
                return null;

            if (!string.IsNullOrEmpty(userId))
            {
                return await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }
            else
            {
                return await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                    .FirstOrDefaultAsync(c => c.SessionId == sessionId);
            }
        }

        public async Task<IActionResult> Index()
        {
            var cart = await GetCart();
            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            ViewData["Title"] = "Thanh toán";
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string fullName, string phoneNumber, string shippingAddress, string paymentMethod, string? customerNote, string? promoCode)
        {
            var cart = await GetCart();
            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var userId = HttpContext.Session.GetString("UserId");
            
            // Calculate totals
            decimal totalAmount = cart.CartItems.Sum(ci => (ci.ProductVariant.Product.SalePrice ?? ci.ProductVariant.Product.BasePrice) * ci.Quantity);
            decimal discountAmount = 0;
            int? promotionId = null;

            if (!string.IsNullOrEmpty(promoCode))
            {
                var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Code == promoCode && p.IsActive && p.StartDate <= DateTime.Now && p.ExpirationDate >= DateTime.Now);
                if (promotion != null && totalAmount >= promotion.MinOrderValue)
                {
                    promotionId = promotion.Id;
                    if (promotion.DiscountPercent.HasValue)
                    {
                        discountAmount = totalAmount * ((decimal)promotion.DiscountPercent.Value / 100);
                    }
                    else if (promotion.DiscountAmount.HasValue)
                    {
                        discountAmount = promotion.DiscountAmount.Value;
                    }
                }
            }

            decimal finalAmount = totalAmount - discountAmount;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create Order
                var order = new Order
                {
                    UserId = userId,
                    PromotionId = promotionId,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    DiscountAmount = discountAmount,
                    FinalAmount = finalAmount,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = "PENDING",
                    OrderStatus = "PENDING",
                    OrderType = "ONLINE",
                    ShippingAddress = $"{fullName} | {phoneNumber} | {shippingAddress}",
                    CustomerNote = customerNote,
                    UpdatedAt = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create Order Details and Update Inventory
                foreach (var item in cart.CartItems)
                {
                    // Reload variant to get latest stock in transaction
                    var variant = await _context.ProductVariants
                        .Include(v => v.Product)
                        .FirstOrDefaultAsync(v => v.Id == item.ProductVariantId);

                    if (variant == null || variant.StockQuantity < item.Quantity)
                    {
                        throw new Exception($"Sản phẩm {variant?.Sku ?? "không xác định"} đã hết hàng hoặc không đủ số lượng.");
                    }

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductVariantId = variant.Id,
                        ProductName = variant.Product.Name,
                        Sku = variant.Sku,
                        Size = variant.Size,
                        Color = variant.Color,
                        Quantity = item.Quantity,
                        UnitPrice = variant.Product.SalePrice ?? variant.Product.BasePrice
                    };
                    _context.OrderDetails.Add(orderDetail);

                    // Update Stock
                    variant.StockQuantity -= item.Quantity;
                    variant.UpdatedAt = DateTime.Now;
                }

                // Clear Cart
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                // Send notification
                var email = HttpContext.Session.GetString("UserEmail") ?? "customer@example.com";
                await _emailService.SendEmailAsync(email, "Xác nhận đơn hàng #" + order.Id, $"Cảm ơn bạn đã đặt hàng. Đơn hàng của bạn đang được xử lý.");

                return RedirectToAction("Success", new { id = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        public async Task<IActionResult> Success(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> ValidatePromo(string code, decimal currentTotal)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Code == code && p.IsActive && p.StartDate <= DateTime.Now && p.ExpirationDate >= DateTime.Now);
            
            if (promotion == null)
                return Json(new { success = false, message = "Mã giảm giá không tồn tại hoặc đã hết hạn." });

            if (currentTotal < promotion.MinOrderValue)
                return Json(new { success = false, message = $"Đơn hàng tối thiểu {promotion.MinOrderValue:N0}đ để sử dụng mã này." });

            decimal discount = 0;
            if (promotion.DiscountPercent.HasValue)
            {
                discount = currentTotal * ((decimal)promotion.DiscountPercent.Value / 100);
            }
            else if (promotion.DiscountAmount.HasValue)
            {
                discount = promotion.DiscountAmount.Value;
            }

            return Json(new { success = true, discount, finalTotal = currentTotal - discount });
        }
    }
}
