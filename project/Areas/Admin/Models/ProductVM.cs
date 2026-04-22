using Microsoft.AspNetCore.Http; // Bắt buộc có để sử dụng IFormFile
using Microsoft.AspNetCore.Mvc.Rendering;
using project.Models;
using System.Collections.Generic;

namespace project.Areas.Admin.Models
{
    public class ProductVM
    {
        public Product Product { get; set; } = new Product();

        // 1. Quản lý Danh mục (Đã có)
        public IEnumerable<SelectListItem>? CategoryList { get; set; }

        // 2. Cập nhật hình ảnh sản phẩm (Use Case: Include - Cập nhật hình ảnh)
        // Cho phép upload một hoặc nhiều ảnh cùng lúc
        public List<IFormFile>? ImageFiles { get; set; }

        // 3. Quản lý phân loại Size và Màu (Use Case: Include - Quản lý phân loại)
        // Danh sách để Admin chọn khi tạo biến thể (Variant)
        public IEnumerable<SelectListItem>? SizeList { get; set; }
        public IEnumerable<SelectListItem>? ColorList { get; set; }

        // Danh sách các biến thể hiện có hoặc chuẩn bị thêm mới
        public List<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    }
}