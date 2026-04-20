create database ShoesShop
-- use ShoesShop;

CREATE TABLE [categories] (
    [id] INT PRIMARY KEY IDENTITY(1,1),
    [name] NVARCHAR(255) NOT NULL
);

CREATE TABLE [user] (
    [id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [role] NVARCHAR(20) NOT NULL CHECK ([role] IN ('admin', 'user', 'pos')),
    [username] NVARCHAR(100) NOT NULL UNIQUE,
    [hashedPassword] NVARCHAR(MAX) NOT NULL,
    [email] NVARCHAR(255) NOT NULL UNIQUE,
    [fullName] NVARCHAR(255) NOT NULL,
    [avatar] NVARCHAR(MAX),
    [createdAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [updatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- soft delete
    [isDeleted] BIT NOT NULL DEFAULT 0 
);

CREATE TABLE [product] (
    [id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [category_id] INT NOT NULL,
    [image] NVARCHAR(MAX) NOT NULL,
    [price] DECIMAL(18, 2) NOT NULL CHECK ([price] >= 0),
    [sale] BIT NOT NULL DEFAULT 0,
    [saleoff] FLOAT NOT NULL DEFAULT 0,
    [total] INT NOT NULL DEFAULT 0,
    [selled] INT NOT NULL DEFAULT 0,
    [available] BIT NOT NULL DEFAULT 1,
    
    -- soft delete
    [isDeleted] BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Product_Category FOREIGN KEY ([category_id]) 
        REFERENCES [categories]([id]) 
        ON DELETE NO ACTION
);

CREATE TABLE [order] (
    [id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [userid] UNIQUEIDENTIFIER NOT NULL,
    [createdAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- soft delete
    [isDeleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Order_User FOREIGN KEY ([userid]) 
        REFERENCES [user]([id]) 
        ON DELETE NO ACTION
);