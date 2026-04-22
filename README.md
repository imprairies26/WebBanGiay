# KINETIC Shoe Shop
---

## Hướng dẫn thiết lập

Bước 1: Cài đặt các gói NuGet (NuGet Packages)
Để EF Core có thể làm việc với SQL Server và tự động sinh code (scaffolding), bạn cần cài đặt một số gói thư viện.
Trong Visual Studio, nhấp chuột phải vào tên dự án của bạn trong cửa sổ Solution Explorer.
Chọn Manage NuGet Packages...
Chuyển sang tab Browse, tìm kiếm và cài đặt 3 gói sau (nhớ chọn phiên bản khớp với phiên bản .NET bạn đang dùng, ví dụ .NET 8 thì cài bản 8.x.x):
Microsoft.EntityFrameworkCore.SqlServer (Để làm việc với SQL Server)
Microsoft.EntityFrameworkCore.Tools (Cung cấp các lệnh cho Package Manager Console)
Microsoft.EntityFrameworkCore.Design (Cần thiết cho quá trình tự động sinh code)
Bước 2: Tự động sinh Code từ Database (Scaffolding)
Quá trình này sẽ đọc cấu trúc Database của bạn và tự động tạo ra các class Models (đại diện cho các bảng) và 1 file DbContext (đại diện cho database).
Trên thanh menu của Visual Studio, chọn Tools > NuGet Package Manager > Package Manager Console.
Tại cửa sổ Console vừa mở ra ở dưới cùng, hãy chạy câu lệnh sau (nhớ thay thế thông tin cho phù hợp với database của bạn):

PowerShell


Scaffold-DbContext "Server=TÊN_SERVER_CỦA_BẠN;Database=TÊN_DATABASE;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models


Giải thích:
Server=...: Tên server SQL của bạn (ví dụ localhost, .\SQLEXPRESS, hoặc IP).
Database=...: Tên cơ sở dữ liệu bạn muốn kết nối.
Trusted_Connection=True: Sử dụng quyền xác thực của Windows (nếu bạn dùng tài khoản sa thì đổi thành User Id=sa;Password=mật_khẩu;).
TrustServerCertificate=True: Tránh lỗi chứng chỉ SSL khi chạy ở môi trường local.
-OutputDir Models: Yêu cầu EF Core đặt tất cả các file class sinh ra vào thư mục Models trong dự án.
Sau khi chạy xong, bạn sẽ thấy thư mục Models có chứa các class tương ứng với các bảng và một file tên là <Tên_Database>Context.cs.
Bước 3: Cấu hình Chuỗi kết nối (Connection String)
Mặc định, lệnh Scaffold sẽ nhúng thẳng chuỗi kết nối vào file DbContext (trong hàm OnConfiguring). Điều này không an toàn. Bạn nên chuyển nó vào file cấu hình.
Mở file appsettings.json và thêm chuỗi kết nối vào như sau:

JSON


{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TÊN_SERVER_CỦA_BẠN;Database=TÊN_DATABASE;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    // ... code mặc định ...
  },
  "AllowedHosts": "*"
}


Mở file DbContext vừa được sinh ra trong thư mục Models (ví dụ MyDatabaseContext.cs), tìm đến hàm OnConfiguring và xóa (hoặc comment) nội dung bên trong hàm đó cùng dòng cảnh báo #warning.
Bước 4: Đăng ký DbContext vào hệ thống (Dependency Injection)
Để có thể sử dụng DbContext ở bất kỳ đâu trong ứng dụng MVC, bạn cần đăng ký nó trong file khởi chạy.
Mở file Program.cs (với các bản .NET 6 trở lên) và thêm đoạn code sau vào trước dòng var app = builder.Build();:

C#


using Microsoft.EntityFrameworkCore;
using Tên_Project_Của_Bạn.Models; // Thay bằng namespace thư mục Models của bạn

var builder = WebApplication.CreateBuilder(args);

// ... code mặc định ...

// Thêm đoạn đăng ký DbContext này:
builder.Services.AddDbContext<MyDatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


(Lưu ý: Thay MyDatabaseContext bằng tên file Context thực tế mà EF Core đã tạo ra cho bạn).
Bước 5: Sử dụng Database trong Controller
Giờ đây, bạn có thể gọi Database vào bất kỳ Controller nào để truy vấn dữ liệu thông qua Dependency Injection (DI).
Ví dụ trong HomeController.cs:

C#


using Microsoft.AspNetCore.Mvc;
using Tên_Project_Của_Bạn.Models;

public class HomeController : Controller
{
    private readonly MyDatabaseContext _context;

    // Inject DbContext qua Constructor
    public HomeController(MyDatabaseContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Lấy danh sách dữ liệu từ một bảng (ví dụ bảng Products)
        var products = _context.Products.ToList();
        
        return View(products);
    }
}


Vậy là hoàn tất! Bạn đã kết nối thành công dự án ASP.NET Core MVC với SQL Server bằng phương pháp Database First và hoàn toàn có thể bắt đầu lập trình các chức năng Thêm/Sửa/Xóa dữ liệu. Nếu sau này Database có thay đổi (thêm cột, thêm bảng), bạn chỉ cần chạy lại câu lệnh Scaffold-DbContext ở Bước 2 kèm theo hậu tố -Force ở cuối lệnh để nó ghi đè lại file Models mới.

Cách cập nhật lại DB vào dự án
Mở lại cửa sổ Package Manager Console (vào Tools > NuGet Package Manager > Package Manager Console).
Chạy lại câu lệnh sau (đảm bảo chuỗi kết nối khớp với dự án của bạn):
PowerShell
Scaffold-DbContext "Server=DESKTOP-L1VGBC9\SQLEXPRESS;Database=ShoesShop;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force

Tham số -Force sẽ báo cho EF Core biết: "Hãy ghi đè (overwrite) lên tất cả các file Models và file DbContext đang có trong thư mục Models".


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

