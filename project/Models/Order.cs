using System;
using System.Collections.Generic;

namespace project.Models;

public partial class Order
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string? StaffId { get; set; }

    public int? PromotionId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;

    public string OrderType { get; set; } = null!;

    public string? ShippingAddress { get; set; }

    public string? CustomerNote { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Promotion? Promotion { get; set; }

    public virtual User? Staff { get; set; }

    public virtual User? User { get; set; }
}
