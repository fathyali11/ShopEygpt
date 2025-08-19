namespace Web.Entites.ViewModels.OrderVMs;

public class OrderDetailsReponseVM
{
    public int Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal TotalPrice { get; init; }
    public DateTime CreatedAt { get; init; }
    public string PaymentIntentId { get; init; } = string.Empty;
    public string StripeSessionId { get; init; } = string.Empty;
    public List<OrderItemDetailsVM> OrderItems { get; init; } = [];

}