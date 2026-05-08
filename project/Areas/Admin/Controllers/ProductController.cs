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
            foreach (var key in ModelState.Keys.ToList())
            {
                if (key.EndsWith(".Product") || key.EndsWith(".Sku") || key.EndsWith(".CartItems") || key.EndsWith(".OrderDetails"))
                {
                    ModelState.Remove(key);
                }
            }

            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;
                product.IsActive = product.IsActive; // It's bound from view

                // The view submits ProductImages and ProductVariants mapped within product object.
                if (product.ProductImages != null)
                {
                    for (int i = 0; i < product.ProductImages.Count; i++)
                    {
                        var img = product.ProductImages.ElementAt(i);
                        img.SortOrder = i;
                    }
                }

                if (product.ProductVariants != null)
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
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", product.CategoryId);
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
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", product.CategoryId);
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
                if (key.EndsWith(".Product") || key.EndsWith(".Sku") || key.EndsWith(".CartItems") || key.EndsWith(".OrderDetails"))
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

                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.BasePrice = product.BasePrice;
                    existingProduct.SalePrice = product.SalePrice;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.IsFeatured = product.IsFeatured;
                    existingProduct.IsActive = product.IsActive;
                    existingProduct.UpdatedAt = DateTime.Now;

                    // Update Images
                    _context.ProductImages.RemoveRange(existingProduct.ProductImages);
                    if (product.ProductImages != null)
                    {
                        for (int i = 0; i < product.ProductImages.Count; i++)
                        {
                            var img = product.ProductImages.ElementAt(i);
                            img.Id = 0; // reset ID to add new
                            img.ProductId = id;
                            img.SortOrder = i;
                            existingProduct.ProductImages.Add(img);
                        }
                    }

                    // Update Variants
                    _context.ProductVariants.RemoveRange(existingProduct.ProductVariants);
                    if (product.ProductVariants != null)
                    {
                        foreach (var variant in product.ProductVariants)
                        {
                            variant.Id = 0; // reset ID to add new
                            variant.ProductId = id;
                            variant.UpdatedAt = DateTime.Now;
                            if (string.IsNullOrEmpty(variant.Sku))
                            {
                                variant.Sku = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                            }
                            existingProduct.ProductVariants.Add(variant);
                        }
                    }

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            var categoriesList = await _context.Categories.ToListAsync();
            ViewBag.Categories = categoriesList;
            ViewBag.CategoryId = new SelectList(categoriesList, "Id", "Name", product.CategoryId);
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
