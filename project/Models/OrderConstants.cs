namespace project.Models;

public static class OrderConstants
{
    public static class Status
    {
        public const string PENDING = "PENDING";
        public const string PROCESSING = "PROCESSING";
        public const string SHIPPED = "SHIPPED";
        public const string COMPLETED = "COMPLETED";
        public const string CANCELLED = "CANCELLED";
    }

    public static class Payment
    {
        public const string UNPAID = "UNPAID";
        public const string PENDING = "PENDING";
        public const string PAID = "PAID";
        public const string REFUNDED = "REFUNDED";
    }

    public static class Type
    {
        public const string ONLINE = "ONLINE";
        public const string POS = "POS";
    }

    public static string GetBadgeClass(string status)
    {
        return status switch
        {
            Status.PENDING or Payment.PENDING or Payment.UNPAID => "bg-warning text-dark",
            Status.PROCESSING or Status.SHIPPED => "bg-info text-white",
            Status.COMPLETED or Payment.PAID => "bg-success text-white",
            Status.CANCELLED or Payment.REFUNDED => "bg-danger text-white",
            _ => "bg-secondary text-white"
        };
    }
}
