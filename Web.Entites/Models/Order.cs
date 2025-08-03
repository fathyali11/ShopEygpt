namespace Web.Entites.Models;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string PaymentIntentId { get; set; } = string.Empty;
    public string StripeSessionId { get; set; }=string.Empty;

    public decimal TotalPrice { get; set; }
    public string Status { get; set; } =string.Empty;

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
