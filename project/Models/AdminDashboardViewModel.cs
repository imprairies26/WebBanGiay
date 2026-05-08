using System.Collections.Generic;

namespace project.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OnlineOrders { get; set; }
        public int PosOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}
