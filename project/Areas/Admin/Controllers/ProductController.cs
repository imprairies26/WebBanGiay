using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project.Areas.Admin.Models;
using project.Models;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public partial class ProductController : Controller
    {
        private readonly ShoesShopContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment; // Dùng để xử lý đường dẫn lưu file ảnh

        public ProductController(ShoesShopContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        // 1. DANH SÁCH + TÌM KIẾM + LỌC (Use Case: Tìm kiếm sản phẩm)
        public async Task<IActionResult> Index(string? searchString, int? categoryId)
        {
            IQueryable<Product> productQuery = _db.Products.Include(p => p.Category).Include(p => p.ProductImages);

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(searchString))
            {
                productQuery = productQuery.Where(p => p.Name.Contains(searchString));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue && categoryId > 0)
            {
                productQuery = productQuery.Where(p => p.CategoryId == categoryId);
            }

            // Gửi danh sách Category sang View để hiển thị ở thanh lọc
            ViewBag.CategoryList = await _db.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
                Selected = c.Id == categoryId
            }).ToListAsync();

            return View(await productQuery.ToListAsync());
        }

        // 2. THÊM MỚI (GET)
        public IActionResult Create()
        {
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = _db.Categories.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() })
            };
            return View(productVM);
        }

        // 3. THÊM MỚI (POST) + UPLOAD ẢNH (Use Case: Cập nhật hình ảnh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM obj)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(obj.Product);
                await _db.SaveChangesAsync();

                // Xử lý upload nhiều ảnh nếu có
                if (obj.ImageFiles != null && obj.ImageFiles.Count > 0)
                {
                    await HandleImageUpload(obj.ImageFiles, obj.Product.Id);
                }

                return RedirectToAction(nameof(Index));
            }

            obj.CategoryList = _db.Categories.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            return View(obj);
        }

        // 4. CHỈNH SỬA (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            ProductVM productVM = new()
            {
                Product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id),
                CategoryList = _db.Categories.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() })
            };

            if (productVM.Product == null) return NotFound();
            return View(productVM);
        }

        // 5. CHỈNH SỬA (POST) + UPLOAD ẢNH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM obj)
        {
            if (ModelState.IsValid)
            {
                obj.Product.UpdatedAt = DateTime.Now;
                _db.Products.Update(obj.Product);
                await _db.SaveChangesAsync();

                // Xử lý upload thêm ảnh mới
                if (obj.ImageFiles != null && obj.ImageFiles.Count > 0)
                {
                    await HandleImageUpload(obj.ImageFiles, obj.Product.Id);
                }

                return RedirectToAction(nameof(Index));
            }

            obj.CategoryList = _db.Categories.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            return View(obj);
        }

        // 6. XÓA (POST) + XÓA FILE VẬT LÝ (Use Case: Xóa sản phẩm)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            // 1. Xóa file ảnh trong thư mục wwwroot trước
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            foreach (var img in product.ProductImages)
            {
                var oldImagePath = Path.Combine(wwwRootPath, img.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // 2. Xóa dữ liệu trong Database
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // --- HÀM TRỢ GIÚP (PRIVATE HELPER) ---
        private async Task HandleImageUpload(List<IFormFile> files, int productId)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string productPath = Path.Combine(wwwRootPath, @"images\products");

            if (!Directory.Exists(productPath)) Directory.CreateDirectory(productPath);

            foreach (var file in files)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                _db.ProductImages.Add(new ProductImage
                {
                    ProductId = productId,
                    ImageUrl = @"\images\products\" + fileName
                });
            }
            await _db.SaveChangesAsync();
        }
    }
}