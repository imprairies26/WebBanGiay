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

                    // 1. Sync Images
                    var incomingImageIds = product.ProductImages?.Select(i => i.Id).Where(id => id > 0).ToList() ?? new List<int>();
                    var imagesToRemove = existingProduct.ProductImages.Where(i => !incomingImageIds.Contains(i.Id)).ToList();
                    _context.ProductImages.RemoveRange(imagesToRemove);

                    if (product.ProductImages != null)
                    {
                        int sortOrder = 0;
                        foreach (var incomingImg in product.ProductImages)
                        {
                            if (incomingImg.Id > 0)
                            {
                                // Update existing
                                var existingImg = existingProduct.ProductImages.FirstOrDefault(i => i.Id == incomingImg.Id);
                                if (existingImg != null)
                                {
                                    existingImg.ImageUrl = incomingImg.ImageUrl;
                                    existingImg.IsMain = incomingImg.IsMain;
                                    existingImg.SortOrder = sortOrder++;
                                }
                            }
                            else
                            {
                                // Add new
                                existingProduct.ProductImages.Add(new ProductImage
                                {
                                    ImageUrl = incomingImg.ImageUrl,
                                    IsMain = incomingImg.IsMain,
                                    SortOrder = sortOrder++
                                });
                            }
                        }
                    }

                    // 2. Sync Variants
                    var incomingVariantIds = product.ProductVariants?.Select(v => v.Id).Where(id => id > 0).ToList() ?? new List<int>();
                    var variantsToRemove = existingProduct.ProductVariants.Where(v => !incomingVariantIds.Contains(v.Id)).ToList();
                    
                    // Only remove variants if they are not in use (or let DB handle error if strict sync is required)
                    // Given the FK error, we should be careful here. 
                    // For now, we attempt to remove them as requested, but we use a more granular update for existing ones.
                    _context.ProductVariants.RemoveRange(variantsToRemove);

                    if (product.ProductVariants != null)
                    {
                        foreach (var incomingVar in product.ProductVariants)
                        {
                            if (incomingVar.Id > 0)
                            {
                                // Update existing
                                var existingVar = existingProduct.ProductVariants.FirstOrDefault(v => v.Id == incomingVar.Id);
                                if (existingVar != null)
                                {
                                    existingVar.Size = incomingVar.Size;
                                    existingVar.Color = incomingVar.Color;
                                    existingVar.StockQuantity = incomingVar.StockQuantity;
                                    existingVar.UpdatedAt = DateTime.Now;
                                    // Keep SKU or update if empty
                                    if (string.IsNullOrEmpty(existingVar.Sku))
                                    {
                                        existingVar.Sku = string.IsNullOrEmpty(incomingVar.Sku) 
                                            ? Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper() 
                                            : incomingVar.Sku;
                                    }
                                }
                            }
                            else
                            {
                                // Add new
                                existingProduct.ProductVariants.Add(new ProductVariant
                                {
                                    Size = incomingVar.Size,
                                    Color = incomingVar.Color,
                                    StockQuantity = incomingVar.StockQuantity,
                                    Sku = string.IsNullOrEmpty(incomingVar.Sku) 
                                        ? Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper() 
                                        : incomingVar.Sku,
                                    UpdatedAt = DateTime.Now
                                });
                            }
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
