using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ShoesShopContext _context;

        public ProductController(ShoesShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            ViewData["Title"] = "Quản lý sản phẩm";
            ViewData["ActiveNav"] = "Products";

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Thêm sản phẩm";
            ViewData["ActiveNav"] = "Products";
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ModelState.Remove("Category");
            // Remove validation for navigation properties to avoid IsValid = false
            foreach (var key in ModelState.Keys.ToList())
            {
                if (key.Contains(".Product") || key.EndsWith(".Sku") || key.Contains(".CartItems") || key.Contains(".OrderDetails"))
                {
                    ModelState.Remove(key);
                }
            }

            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;

                // Process Images
                if (product.ProductImages != null && product.ProductImages.Any())
                {
                    int i = 0;
                    foreach (var img in product.ProductImages)
                    {
                        img.SortOrder = i++;
                    }
                }

                // Process Variants
                if (product.ProductVariants != null && product.ProductVariants.Any())
                {
                    foreach (var variant in product.ProductVariants)
                    {
                        variant.UpdatedAt = DateTime.Now;
                        if (string.IsNullOrEmpty(variant.Sku))
                        {
                            variant.Sku = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                        }
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ViewData["Title"] = "Chỉnh sửa sản phẩm";
            ViewData["ActiveNav"] = "Products";
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            ModelState.Remove("Category");
            foreach (var key in ModelState.Keys.ToList())
            {
                if (key.Contains(".Product") || key.EndsWith(".Sku") || key.Contains(".CartItems") || key.Contains(".OrderDetails"))
                {
                    ModelState.Remove(key);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .FirstOrDefaultAsync(p => p.Id == id);
                        
                    if (existingProduct == null) return NotFound();

                    // Update basic info
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.BasePrice = product.BasePrice;
                    existingProduct.SalePrice = product.SalePrice;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.IsFeatured = product.IsFeatured;
                    existingProduct.IsActive = product.IsActive;
                    existingProduct.UpdatedAt = DateTime.Now;

                    // Update Images: Clear and Re-add (Safely)
                    _context.ProductImages.RemoveRange(existingProduct.ProductImages);
                    if (product.ProductImages != null)
                    {
                        int i = 0;
                        foreach (var img in product.ProductImages)
                        {
                            existingProduct.ProductImages.Add(new ProductImage 
                            { 
                                ProductId = id,
                                ImageUrl = img.ImageUrl,
                                IsMain = img.IsMain,
                                SortOrder = i++
                            });
                        }
                    }

                    // Update Variants: Clear and Re-add (Safely)
                    _context.ProductVariants.RemoveRange(existingProduct.ProductVariants);
                    if (product.ProductVariants != null)
                    {
                        foreach (var variant in product.ProductVariants)
                        {
                            existingProduct.ProductVariants.Add(new ProductVariant
                            {
                                ProductId = id,
                                Size = variant.Size,
                                Color = variant.Color,
                                Sku = string.IsNullOrEmpty(variant.Sku) ? Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper() : variant.Sku,
                                StockQuantity = variant.StockQuantity,
                                UpdatedAt = DateTime.Now
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Soft delete
                product.IsActive = false;
                product.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
