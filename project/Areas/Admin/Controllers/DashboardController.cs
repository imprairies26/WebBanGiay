using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ShoesShopContext _context;

        public DashboardController(ShoesShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["ActiveNav"] = "Dashboard";

            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalOrders = await _context.Orders.CountAsync(),
                CompletedOrders = await _context.Orders.CountAsync(o => o.OrderStatus == "COMPLETED"),
                TotalRevenue = await _context.Orders.Where(o => o.OrderStatus == "COMPLETED").SumAsync(o => o.FinalAmount),
                OnlineOrders = await _context.Orders.CountAsync(o => o.OrderType == "ONLINE"),
                PosOrders = await _context.Orders.CountAsync(o => o.OrderType == "POS"),
                TotalCustomers = await _context.Users.CountAsync(u => u.RoleId == 3),
                TotalProducts = await _context.Products.CountAsync(),
                RecentOrders = recentOrders
            };

            return View(viewModel);
        }
    }
}
