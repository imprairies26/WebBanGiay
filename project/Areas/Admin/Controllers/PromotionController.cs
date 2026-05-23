using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PromotionController : Controller
    {
        private readonly ShoesShopContext _context;

        public PromotionController(ShoesShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Quản lý mã giảm giá";
            ViewData["ActiveNav"] = "Promotions";
            var promotions = await _context.Promotions.OrderByDescending(p => p.StartDate).ToListAsync();
            return View(promotions);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo mã giảm giá";
            ViewData["ActiveNav"] = "Promotions";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                promotion.UsedCount = 0;
                _context.Add(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) return NotFound();

            ViewData["Title"] = "Chỉnh sửa mã giảm giá";
            ViewData["ActiveNav"] = "Promotions";
            return View(promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Promotion promotion)
        {
            if (id != promotion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promotion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promotion.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.Id == id);
        }
    }
}
