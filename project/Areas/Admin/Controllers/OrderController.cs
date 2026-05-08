using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ShoesShopContext _context;

        public OrderController(ShoesShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, string? status)
        {
            ViewData["Title"] = "Quản lý đơn hàng";
            ViewData["ActiveNav"] = "Orders";

            var query = _context.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.Id.ToString().Contains(search) || 
                                        (o.User != null && o.User.FullName.Contains(search)) ||
                                        (o.User != null && o.User.Email.Contains(search)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Detail(int id)
        {
            ViewData["Title"] = "Chi tiết đơn hàng";
            ViewData["ActiveNav"] = "Orders";

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string orderStatus, string paymentStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                if (!string.IsNullOrEmpty(orderStatus))
                {
                    order.OrderStatus = orderStatus;
                }
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    order.PaymentStatus = paymentStatus;
                }
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Detail), new { id = id });
        }
    }
}
