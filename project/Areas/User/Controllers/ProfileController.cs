using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.User.Controllers
{
    [Area("User")]
    public class ProfileController : Controller
    {
        private readonly ShoesShopContext _context;

        public ProfileController(ShoesShopContext context)
        {
            _context = context;
        }

        private string? GetUserId() => HttpContext.Session.GetString("UserId");

        public async Task<IActionResult> Index(string tab = "orders")
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account", new { area = "" });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.ActiveTab = tab;
            ViewBag.Orders = orders;

            return View(user);
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account", new { area = "" });

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                            .ThenInclude(p => p.ProductImages)
                .Include(o => o.Promotion)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo(string fullName, string? phoneNumber)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account", new { area = "" });

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.FullName = fullName;
                user.PhoneNumber = phoneNumber;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                // Update Session for Header sync
                HttpContext.Session.SetString("UserFullName", user.FullName);

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
            }

            return RedirectToAction(nameof(Index), new { tab = "settings" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(string avatarUrl)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account", new { area = "" });

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.AvatarUrl = avatarUrl;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                // Update Session for Header sync
                HttpContext.Session.SetString("UserAvatar", user.AvatarUrl ?? "");

                TempData["SuccessMessage"] = "Cập nhật ảnh đại diện thành công.";
            }

            return RedirectToAction(nameof(Index), new { tab = "settings" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account", new { area = "" });

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Mật khẩu hiện tại không chính xác.";
                }
            }

            return RedirectToAction(nameof(Index), new { tab = "security" });
        }
    }
}
