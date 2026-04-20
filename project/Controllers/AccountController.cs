using Microsoft.AspNetCore.Mvc;
using project.Models;
using System.Text.RegularExpressions;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace project.Controllers
{
    public class AccountController : Controller
    {
        private readonly ShoesShopContext _context;

        public AccountController(ShoesShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home", new { area = "User" });
            }
            ViewData["Title"] = "Đăng nhập - KINETIC";
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Tìm user theo Email (Bản DB mới dùng Email làm định danh duy nhất)
                var user = await _context.Users.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Identifier && u.IsActive);

                if (user != null)
                {
                    // 2. Xác minh mật khẩu
                    if (BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                    {
                        // 3. Lưu thông tin vào Session
                        HttpContext.Session.SetString("UserId", user.Id);
                        HttpContext.Session.SetString("UserRole", user.Role?.Name ?? "Customer");
                        HttpContext.Session.SetString("UserRoleId", user.RoleId.ToString());
                        HttpContext.Session.SetString("UserFullName", user.FullName);
                        HttpContext.Session.SetString("UserAvatar", user.AvatarUrl ?? "");

                        TempData["SuccessMessage"] = $"Chào mừng {user.FullName} trở lại!";

                        // 4. Điều hướng theo Role Id (1: Admin, 2: Staff/POS, 3: Customer)
                        if (user.RoleId == 1) return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                        if (user.RoleId == 2) return RedirectToAction("Index", "Sale", new { area = "POS" });
                        return RedirectToAction("Index", "Home", new { area = "User" });
                    }
                }
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Đăng ký thành viên - KINETIC";
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // Mã hóa mật khẩu
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(), // ID string khớp NVARCHAR(450)
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = passwordHash,
                    RoleId = 3, // Mặc định là Customer
                    AvatarUrl = "https://ui-avatars.com/api/?name=" + Uri.EscapeDataString(model.FullName) + "&background=random",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công! Hãy đăng nhập.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home", new { area = "User" });
        }
    }
}
