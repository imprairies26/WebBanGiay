USE master;
GO

-- 1. XỬ LÝ DATABASE
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ShoesShop')
BEGIN
    ALTER DATABASE ShoesShop SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ShoesShop;
END
CREATE DATABASE ShoesShop;
GO

USE ShoesShop;
GO

IF OBJECT_ID('OrderDetails', 'U') IS NOT NULL DROP TABLE OrderDetails;
IF OBJECT_ID('Orders', 'U')       IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('CartItems', 'U')    IS NOT NULL DROP TABLE CartItems;
IF OBJECT_ID('Carts', 'U')        IS NOT NULL DROP TABLE Carts;
IF OBJECT_ID('Promotions', 'U')   IS NOT NULL DROP TABLE Promotions;
IF OBJECT_ID('ProductVariants', 'U') IS NOT NULL DROP TABLE ProductVariants;
IF OBJECT_ID('ProductImages', 'U')   IS NOT NULL DROP TABLE ProductImages;
IF OBJECT_ID('Products', 'U')     IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Banners', 'U')      IS NOT NULL DROP TABLE Banners;
IF OBJECT_ID('Categories', 'U')   IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('PasswordResetTokens', 'U') IS NOT NULL DROP TABLE PasswordResetTokens;
IF OBJECT_ID('Users', 'U')        IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Roles', 'U')        IS NOT NULL DROP TABLE Roles;
GO

--NGƯỜI DÙNG VÀ PHÂN QUYỀN
CREATE TABLE Roles (
    Id   INT IDENTITY(1, 1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE -- Admin | Staff | Customer
);

-- Seed dữ liệu Role mặc định
INSERT INTO Roles (Name) VALUES ('Admin'), ('Staff'), ('Customer');

CREATE TABLE Users (
    Id           NVARCHAR(450) NOT NULL PRIMARY KEY,
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(256) NOT NULL UNIQUE,
    PhoneNumber  NVARCHAR(20)  NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    AvatarUrl    NVARCHAR(500) NULL,
    RoleId       INT           NOT NULL DEFAULT 3,
    IsActive     BIT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE(),
    UpdatedAt    DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE INDEX IDX_Users_Email ON Users(Email);
CREATE INDEX IDX_Users_RoleId ON Users(RoleId);

CREATE TABLE PasswordResetTokens (
    Id         INT IDENTITY(1, 1) PRIMARY KEY,
    UserId     NVARCHAR(450) NOT NULL,
    Token      NVARCHAR(10)  NOT NULL,
    ExpiresAt  DATETIME      NOT NULL,
    IsUsed     BIT           NOT NULL DEFAULT 0,
    CreatedAt  DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_PasswordResetTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

--DANH MỤC VÀ SẢN PHẨM
CREATE TABLE Categories (
    Id          INT IDENTITY(1, 1) PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL
);

CREATE TABLE Banners (
    Id          INT IDENTITY(1, 1) PRIMARY KEY,
    ImageUrl    NVARCHAR(500) NOT NULL,
    LinkUrl     NVARCHAR(500) NULL,
    SortOrder   INT           NOT NULL DEFAULT 0,
    IsActive    BIT           NOT NULL DEFAULT 1
);

CREATE TABLE Products (
    Id            INT IDENTITY(1, 1) PRIMARY KEY,
    CategoryId    INT            NOT NULL,
    Name          NVARCHAR(200)  NOT NULL,
    Description   NVARCHAR(MAX)  NULL,
    BasePrice     DECIMAL(18, 2) NOT NULL,
    SalePrice     DECIMAL(18, 2) NULL,
    SaleStartDate DATETIME       NULL,
    SaleEndDate   DATETIME       NULL,
    IsFeatured    BIT            NOT NULL DEFAULT 0,
    IsActive      BIT            NOT NULL DEFAULT 1,
    CreatedAt     DATETIME       NOT NULL DEFAULT GETDATE(),
    UpdatedAt     DATETIME       NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT CHK_Products_SalePrice  CHECK (SalePrice IS NULL OR SalePrice < BasePrice),
    CONSTRAINT CHK_Products_SaleDates  CHECK (SalePrice IS NULL OR (SaleStartDate IS NOT NULL AND SaleEndDate IS NOT NULL AND SaleEndDate > SaleStartDate))
);

CREATE INDEX IDX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IDX_Products_IsActive   ON Products(IsActive);

CREATE TABLE ProductImages (
    Id        INT IDENTITY(1, 1) PRIMARY KEY,
    ProductId INT            NOT NULL,
    ImageUrl  NVARCHAR(500)  NOT NULL,
    IsMain    BIT            NOT NULL DEFAULT 0,
    SortOrder INT            NOT NULL DEFAULT 0,
    CONSTRAINT FK_ProductImages_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

CREATE TABLE ProductVariants (
    Id            INT IDENTITY(1, 1) PRIMARY KEY,
    ProductId     INT           NOT NULL,
    Size          NVARCHAR(20)  NOT NULL,
    Color         NVARCHAR(50)  NOT NULL,
    SKU           NVARCHAR(100) NOT NULL UNIQUE,
    StockQuantity INT           NOT NULL DEFAULT 0,
    UpdatedAt     DATETIME      NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_ProductVariants_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_ProductVariants_Stock    CHECK (StockQuantity >= 0),
    CONSTRAINT UQ_ProductVariants_SizeColor UNIQUE (ProductId, Size, Color)
);

-- KHUYẾN MÃI GIỎ HÀNG
CREATE TABLE Promotions (
    Id              INT IDENTITY(1, 1) PRIMARY KEY,
    Code            NVARCHAR(50)   NOT NULL UNIQUE,
    DiscountPercent INT            NULL,
    DiscountAmount  DECIMAL(18, 2) NULL,
    MinOrderValue   DECIMAL(18, 2) NOT NULL DEFAULT 0,
    StartDate       DATETIME       NOT NULL,
    ExpirationDate  DATETIME       NOT NULL,
    UsageLimit      INT            NULL,
    UsedCount       INT            NOT NULL DEFAULT 0,
    IsActive        BIT            NOT NULL DEFAULT 1,

    CONSTRAINT CHK_Promotions_DiscountType    CHECK ((DiscountPercent IS NOT NULL AND DiscountAmount IS NULL) OR (DiscountPercent IS NULL AND DiscountAmount IS NOT NULL)),
    CONSTRAINT CHK_Promotions_DiscountPercent CHECK (DiscountPercent IS NULL OR (DiscountPercent > 0 AND DiscountPercent <= 100)),
    CONSTRAINT CHK_Promotions_Dates           CHECK (ExpirationDate > StartDate)
);

CREATE TABLE Carts (
    Id        INT IDENTITY(1, 1) PRIMARY KEY,
    UserId    NVARCHAR(450) NULL,
    SessionId NVARCHAR(100) NULL,
    CONSTRAINT FK_Carts_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL,
    CONSTRAINT CHK_Carts_Owner CHECK (UserId IS NOT NULL OR SessionId IS NOT NULL)
);

CREATE TABLE CartItems (
    Id               INT IDENTITY(1, 1) PRIMARY KEY,
    CartId           INT NOT NULL,
    ProductVariantId INT NOT NULL,
    Quantity         INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_CartItems_Carts           FOREIGN KEY (CartId) REFERENCES Carts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CartItems_ProductVariants FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id),
    CONSTRAINT CHK_CartItems_Quantity       CHECK (Quantity > 0),
    CONSTRAINT UQ_CartItems_Cart_Variant    UNIQUE (CartId, ProductVariantId)
);


-- ĐƠN HÀNG
CREATE TABLE Orders (
    Id              INT IDENTITY(1, 1) PRIMARY KEY,
    UserId          NVARCHAR(450)  NULL,
    StaffId         NVARCHAR(450)  NULL,
    PromotionId     INT            NULL,
    OrderDate       DATETIME       NOT NULL DEFAULT GETDATE(),
    TotalAmount     DECIMAL(18, 2) NOT NULL,
    DiscountAmount  DECIMAL(18, 2) NOT NULL DEFAULT 0,
    FinalAmount     DECIMAL(18, 2) NOT NULL,
    PaymentMethod   NVARCHAR(50)   NOT NULL,
    PaymentStatus   NVARCHAR(50)   NOT NULL DEFAULT 'Pending',
    OrderStatus     NVARCHAR(50)   NOT NULL DEFAULT 'Pending',
    OrderType       NVARCHAR(20)   NOT NULL,
    ShippingAddress NVARCHAR(500)  NULL,
    CustomerNote    NVARCHAR(500)  NULL,
    UpdatedAt       DATETIME       NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Orders_Users      FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_Orders_Staff      FOREIGN KEY (StaffId) REFERENCES Users(Id),
    CONSTRAINT FK_Orders_Promotions FOREIGN KEY (PromotionId) REFERENCES Promotions(Id),
    CONSTRAINT CHK_Orders_Amounts   CHECK (TotalAmount >= 0 AND FinalAmount >= 0),
    CONSTRAINT CHK_Orders_Type      CHECK (OrderType IN ('Online', 'POS')),
    CONSTRAINT CHK_Orders_Payment   CHECK (PaymentMethod IN ('COD', 'BankTransfer', 'Cash'))
);

CREATE TABLE OrderDetails (
    Id               INT IDENTITY(1, 1) PRIMARY KEY,
    OrderId          INT            NOT NULL,
    ProductVariantId INT            NOT NULL,
    ProductName      NVARCHAR(200)  NOT NULL,
    SKU              NVARCHAR(100)  NOT NULL,
    Size             NVARCHAR(20)   NOT NULL,
    Color            NVARCHAR(50)   NOT NULL,
    Quantity         INT            NOT NULL,
    UnitPrice        DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_OrderDetails_Orders          FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_ProductVariants FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id),
    CONSTRAINT CHK_OrderDetails_Quantity       CHECK (Quantity > 0)
);
GO


-- STORE PROCEDURE
-- 1. Lấy giá hiện tại
CREATE OR ALTER PROCEDURE GetCurrentPrice 
    @ProductId INT 
AS 
BEGIN
    SELECT 
        Id, Name, BasePrice, SalePrice,
        CASE 
            WHEN SalePrice IS NOT NULL AND GETDATE() BETWEEN SaleStartDate AND SaleEndDate THEN SalePrice 
            ELSE BasePrice 
        END AS CurrentPrice,
        CASE 
            WHEN SalePrice IS NOT NULL AND GETDATE() BETWEEN SaleStartDate AND SaleEndDate THEN 1 
            ELSE 0 
        END AS IsOnSale
    FROM Products
    WHERE Id = @ProductId AND IsActive = 1;
END;
GO

-- 2. Thống kê doanh thu
CREATE OR ALTER PROCEDURE GetRevenueSummary 
    @FromDate DATETIME,
    @ToDate DATETIME 
AS 
BEGIN
    SELECT 
        COUNT(*) AS TotalOrders,
        SUM(CASE WHEN OrderStatus = 'Completed' THEN 1 ELSE 0 END) AS CompletedOrders,
        SUM(CASE WHEN OrderStatus = 'Completed' THEN FinalAmount ELSE 0 END) AS TotalRevenue,
        SUM(CASE WHEN OrderType = 'Online' THEN 1 ELSE 0 END) AS OnlineOrders,
        SUM(CASE WHEN OrderType = 'POS' THEN 1 ELSE 0 END) AS POSOrders
    FROM Orders
    WHERE OrderDate BETWEEN @FromDate AND @ToDate;
END;
GO

-- 3. Kiểm tra khuyến mãi
CREATE OR ALTER PROCEDURE ValidatePromotion 
    @Code NVARCHAR(50),
    @OrderAmount DECIMAL(18, 2) 
AS 
BEGIN
    SELECT 
        Id, Code, DiscountPercent, DiscountAmount, MinOrderValue,
        CASE 
            WHEN IsActive = 0 THEN N'Mã không hoạt động'
            WHEN GETDATE() < StartDate THEN N'Mã chưa đến ngày sử dụng'
            WHEN GETDATE() > ExpirationDate THEN N'Mã đã hết hạn'
            WHEN UsageLimit IS NOT NULL AND UsedCount >= UsageLimit THEN N'Mã đã hết lượt sử dụng'
            WHEN @OrderAmount < MinOrderValue THEN N'Đơn hàng chưa đạt giá trị tối thiểu'
            ELSE N'OK' 
        END AS ValidationMessage
    FROM Promotions
    WHERE Code = @Code;
END;
GO