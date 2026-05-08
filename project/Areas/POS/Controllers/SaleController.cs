using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.POS.Controllers
{
    [Area("POS")]
    public class SaleController : Controller
    {
        private readonly ShoesShopContext _context;

        public SaleController(ShoesShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Bán hàng tại quầy";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchProduct(string query)
        {
            if (string.IsNullOrEmpty(query)) return Json(new List<object>());

            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(v => v.Sku == query || v.Product.Name.Contains(query))
                .Select(v => new {
                    id = v.Id,
                    name = v.Product.Name,
                    size = v.Size,
                    color = v.Color,
                    sku = v.Sku,
                    price = v.Product.SalePrice ?? v.Product.BasePrice,
                    stock = v.StockQuantity,
                    image = v.Product.ProductImages.Any(i => i.IsMain) 
                        ? v.Product.ProductImages.First(i => i.IsMain).ImageUrl 
                        : (v.Product.ProductImages.Any() ? v.Product.ProductImages.First().ImageUrl : "")
                })
                .ToListAsync();

            return Json(variants);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] POSOrderRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
                return Json(new { success = false, message = "Giỏ hàng trống." });

            var staffId = HttpContext.Session.GetString("UserId");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    StaffId = staffId,
                    OrderDate = DateTime.Now,
                    TotalAmount = request.TotalAmount,
                    DiscountAmount = request.DiscountAmount,
                    FinalAmount = request.FinalAmount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = "COMPLETED",
                    OrderStatus = "COMPLETED",
                    OrderType = "POS",
                    CustomerNote = "POS Order",
                    UpdatedAt = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in request.Items)
                {
                    var variant = await _context.ProductVariants.Include(v => v.Product).FirstOrDefaultAsync(v => v.Id == item.VariantId);
                    if (variant == null || variant.StockQuantity < item.Quantity)
                    {
                        throw new Exception($"Sản phẩm {variant?.Sku} không đủ tồn kho.");
                    }

                    var detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductVariantId = variant.Id,
                        ProductName = variant.Product.Name,
                        Sku = variant.Sku,
                        Size = variant.Size,
                        Color = variant.Color,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _context.OrderDetails.Add(detail);

                    // Deduct stock
                    variant.StockQuantity -= item.Quantity;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, orderId = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class POSOrderRequest
    {
        public List<POSItemRequest> Items { get; set; } = new List<POSItemRequest>();
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; } = "CASH";
    }

    public class POSItemRequest
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
