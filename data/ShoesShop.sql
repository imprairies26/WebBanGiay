drop database ShoesShop

CREATE TABLE [User] (
    Id NVARCHAR(450) PRIMARY KEY, -- GUID từ ASP.NET Identity
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(50),
    AvatarUrl NVARCHAR(MAX), -- URL ảnh từ cloud storage
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Category (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX)
);

CREATE TABLE Banner (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ImageUrl NVARCHAR(MAX) NOT NULL,
    LinkUrl NVARCHAR(MAX), -- Chuyển hướng khi click, null = không link
    SortOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Promotion (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL UNIQUE,
    DiscountPercent INT,
    DiscountAmount DECIMAL(18,2),
    MinOrderValue DECIMAL(18,2) NOT NULL DEFAULT 0,
    StartDate DATETIME NOT NULL,
    ExpirationDate DATETIME NOT NULL,
    UsageLimit INT, -- Tổng lượt dùng tối đa, null = vô hạn
    UsedCount INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1
);

-- 2. BẢNG CÓ KHÓA NGOẠI (TẠO SAU)

CREATE TABLE Product (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Category(Id),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    BasePrice DECIMAL(18,2) NOT NULL, -- Giá gốc
    SalePrice DECIMAL(18,2), -- Giá sale, null = không sale
    SaleStartDate DATETIME,
    SaleEndDate DATETIME,
    IsFeatured BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE ProductImage (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Product(Id),
    ImageUrl NVARCHAR(MAX) NOT NULL,
    IsMain BIT NOT NULL DEFAULT 0,
    SortOrder INT NOT NULL DEFAULT 0
);

CREATE TABLE ProductVariant (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Product(Id),
    Size NVARCHAR(50) NOT NULL,
    Color NVARCHAR(50) NOT NULL,
    SKU NVARCHAR(100) NOT NULL UNIQUE,
    StockQuantity INT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Cart (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) FOREIGN KEY REFERENCES [User](Id), -- Null nếu khách chưa đăng nhập
    SessionId NVARCHAR(255) -- Dùng để track giỏ hàng khách vãng lai
);

CREATE TABLE CartItem (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CartId INT NOT NULL FOREIGN KEY REFERENCES Cart(Id),
    ProductVariantId INT NOT NULL FOREIGN KEY REFERENCES ProductVariant(Id),
    Quantity INT NOT NULL DEFAULT 1
);

CREATE TABLE [Order] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) FOREIGN KEY REFERENCES [User](Id),
    StaffId NVARCHAR(450) FOREIGN KEY REFERENCES [User](Id), -- Nhân viên xử lý, chỉ có ở đơn POS
    PromotionId INT FOREIGN KEY REFERENCES Promotion(Id),
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    FinalAmount DECIMAL(18,2) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL, -- COD | BankTransfer | Cash
    PaymentStatus NVARCHAR(50) NOT NULL, -- Pending | Paid | Refunded
    OrderStatus NVARCHAR(50) NOT NULL, -- Pending | Processing | Shipped | Completed | Cancelled
    OrderType NVARCHAR(50) NOT NULL, -- Online | POS
    ShippingAddress NVARCHAR(MAX), -- Null nếu là đơn POS
    CustomerNote NVARCHAR(MAX),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE OrderDetail (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL FOREIGN KEY REFERENCES [Order](Id),
    ProductVariantId INT NOT NULL FOREIGN KEY REFERENCES ProductVariant(Id),
    ProductName NVARCHAR(255) NOT NULL, -- Snapshot
    SKU NVARCHAR(100) NOT NULL, -- Snapshot
    Size NVARCHAR(50), -- Snapshot
    Color NVARCHAR(50), -- Snapshot
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL -- Snapshot
);