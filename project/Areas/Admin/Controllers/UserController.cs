using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ShoesShopContext _context;

        public UserController(ShoesShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewData["Title"] = "Quản lý người dùng";
            ViewData["ActiveNav"] = "Users";

            var query = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
            }

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
            ViewBag.Roles = await _context.Roles.ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string userId, int roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RoleId = roleId;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
