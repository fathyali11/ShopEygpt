namespace Web.Entites.Consts;

public static class OrderStatus
{
    public const string StatusPending = "Pending";//قيد الانتظار
    public const string StatusApproved = "Approved";//موافقه
    public const string StatusInProcess = "Processing";//تعالج
    public const string StatusShipped = "Shipped";//تم شحنها
    public const string StatusCancelled = "Cancelled";//تم الالغاء
    public const string StatusRefunded = "Refunded";//تم ردها

    
}
public static class PaymentStatus
{
    public const string PaymentStatusPaid = "Paid";
    public const string PaymentStatusPending = "Pending";
    public const string PaymentStatusFailed = "Failed";
    public const string PaymentStatusRejected = "Rejected";
}
