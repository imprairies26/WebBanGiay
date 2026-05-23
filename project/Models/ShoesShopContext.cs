using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace project.Models;

public partial class ShoesShopContext : DbContext
{
    public ShoesShopContext()
    {
    }

    public ShoesShopContext(DbContextOptions<ShoesShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductVariant> ProductVariants { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-L1VGBC9\\SQLEXPRESS;Database=ShoesShop;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banners__3214EC0797D4550E");

            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LinkUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carts__3214EC07F9B0E739");

            entity.Property(e => e.SessionId).HasMaxLength(100);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Carts_Users");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC074F11932D");

            entity.HasIndex(e => new { e.CartId, e.ProductVariantId }, "UQ_CartItems_Cart_Variant").IsUnique();

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK_CartItems_Carts");

            entity.HasOne(d => d.ProductVariant).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_ProductVariants");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07C8770C2F");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC072B7B9D7A");

            entity.Property(e => e.CustomerNote).HasMaxLength(500);
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(50)
                .HasDefaultValue("PENDING");
            entity.Property(e => e.OrderType).HasMaxLength(20);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("PENDING");
            entity.Property(e => e.ShippingAddress).HasMaxLength(500);
            entity.Property(e => e.StaffId).HasMaxLength(450);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Promotion).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK_Orders_Promotions");

            entity.HasOne(d => d.Staff).WithMany(p => p.OrderStaffs)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK_Orders_Staff");

            entity.HasOne(d => d.User).WithMany(p => p.OrderUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC0748DBDD58");

            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Size).HasMaxLength(20);
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("SKU");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.ProductVariant).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_ProductVariants");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC071C7AF554");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(10);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PasswordResetTokens_Users");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07A4C19FB4");

            entity.HasIndex(e => e.CategoryId, "IDX_Products_CategoryId");

            entity.HasIndex(e => e.IsActive, "IDX_Products_IsActive");

            entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.SaleEndDate).HasColumnType("datetime");
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SaleStartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC076928AED6");

            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductImages_Products");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductV__3214EC075F8C641D");

            entity.HasIndex(e => new { e.ProductId, e.Size, e.Color }, "UQ_ProductVariants_SizeColor").IsUnique();

            entity.HasIndex(e => e.Sku, "UQ__ProductV__CA1ECF0D05C3A64B").IsUnique();

            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Size).HasMaxLength(20);
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("SKU");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductVariants_Products");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promotio__3214EC0701B9B152");

            entity.HasIndex(e => e.Code, "UQ__Promotio__A25C5AA77AE113EE").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MinOrderValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07881E725C");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F6864436A0").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC079FBBC54C");

            entity.HasIndex(e => e.Email, "IDX_Users_Email");

            entity.HasIndex(e => e.RoleId, "IDX_Users_RoleId");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534E7AD6514").IsUnique();

            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RoleId).HasDefaultValue(3);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
