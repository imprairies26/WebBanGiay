# KINETIC Shoe Shop
---

## Hướng dẫn thiết lập

### 1. Cài đặt Database
Dự án sử dụng SQL Server. Thực hiện các bước sau để thiết lập:

1.  Mở **SQL Server Management Studio (SSMS)**.
2.  Mở tệp script tại đường dẫn: `WebBanGiay/project/Data/ShoesShop.sql`.
3.  Chạy (Execute) toàn bộ script để tạo cơ sở dữ liệu `ShoeShopDB`, các bảng, quan hệ và các Store Procedure cần thiết.

### 2. Cấu hình Chuỗi kết nối
Mở tệp `appsettings.json` trong thư mục gốc và cập nhật chuỗi kết nối khớp với máy của bạn:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TÊN_SERVER;Database=ShoeShopDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Tạo Model từ Database
Dự án sử dụng phương pháp **Database First**. Nếu bạn có thay đổi cấu trúc Database trong SQL Server, hãy chạy lệnh sau trong **Package Manager Console** của Visual Studio:

```powershell (mở tại thư mục project/)
Scaffold-DbContext "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context ShoesShopContext -Force
```

*Lưu ý: Sau khi Scaffold, hãy rà soát lại tệp `Models/User.cs` để đảm bảo các trường bảo mật (PasswordHash, RoleId, v.v.) được cấu hình đúng như trong DB*

---

## Chạy ứng dụng

1.  Chạy ứng dụng
2.  Truy cập:
    - **Cửa hàng:** `https://localhost:PORT/User/Home`
    - **Đăng nhập:** `https://localhost:PORT/Account/Login`

---

## Cấu trúc thư mục Chính
- `Areas/Admin`: Quản lý Sản phẩm, Người dùng, Thống kê.
- `Areas/POS`: Giao diện bán hàng tại quầy.
- `Areas/User`: Giao diện mua sắm cho khách hàng.
- `Controllers`: Chứa logic Xác thực
- `Filters`: Chứa `SessionAuthorizeFilter` để bảo mật hệ thống.
- `wwwroot`: Tài nguyên tĩnh

