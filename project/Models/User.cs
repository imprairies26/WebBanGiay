using System;
using System.Collections.Generic;

namespace project.Models;

public partial class User
{
    public string Id { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> OrderStaffs { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();

    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    public virtual Role Role { get; set; } = null!;
}
