using Microsoft.AspNetCore.Mvc;
using project.Models;
using System.Text.RegularExpressions;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace project.Controllers.Web
{
    public class AccountController : Controller
    {
        private readonly ShoesShopContext _context;
        private readonly project.Services.IEmailService _emailService;

        public AccountController(ShoesShopContext context, project.Services.IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "Quên mật khẩu";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                var token = Guid.NewGuid().ToString();
                var resetToken = new PasswordResetToken
                {
                    UserId = user.Id,
                    Token = token,
                    ExpiresAt = DateTime.Now.AddHours(1),
                    CreatedAt = DateTime.Now,
                    IsUsed = false
                };

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = email }, Request.Scheme);
                await _emailService.SendEmailAsync(email, "Đặt lại mật khẩu KINETIC", $"Click vào link để đặt lại mật khẩu: {resetLink}");
            }

            TempData["SuccessMessage"] = "Nếu email tồn tại trong hệ thống, bạn sẽ nhận được liên kết đặt lại mật khẩu sớm.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var resetToken = await _context.PasswordResetTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.User.Email == email && !rt.IsUsed && rt.ExpiresAt > DateTime.Now);

            if (resetToken == null) return BadRequest("Liên kết không hợp lệ hoặc đã hết hạn.");

            ViewData["Title"] = "Đặt lại mật khẩu";
            ViewBag.Token = token;
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resetToken = await _context.PasswordResetTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == model.Token && rt.User.Email == model.Email && !rt.IsUsed && rt.ExpiresAt > DateTime.Now);

            if (resetToken == null) return BadRequest("Yêu cầu không hợp lệ hoặc link đã hết hạn.");

            resetToken.IsUsed = true;
            resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            resetToken.User.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mật khẩu đã được cập nhật thành công. Hãy đăng nhập với mật khẩu mới.";
            return RedirectToAction("Login");
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
                        HttpContext.Session.SetString("UserEmail", user.Email);
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
            TempData.Clear(); // Xóa TempData để tránh thông báo cũ hiển thị lại
            return RedirectToAction("Index", "Home", new { area = "User" });
        }
    }
}
