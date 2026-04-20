using System;
using System.Collections.Generic;

namespace project.Models;

public partial class User
{
    public string Id { get; set; } = null!; // NVARCHAR(450)

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = null!; // Mã hóa mật khẩu

    public string? AvatarUrl { get; set; }

    public int RoleId { get; set; } = 3; // Mặc định là Customer

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<Order> OrderStaffs { get; set; } = new List<Order>();
    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();
}
