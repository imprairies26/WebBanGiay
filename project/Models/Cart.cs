using System;
using System.Collections.Generic;

namespace project.Models;

public partial class Cart
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string? SessionId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User? User { get; set; }
}
