namespace Web.Entites.ViewModels.OrderVMs;

public class OrderProfileVM
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemProfileVM> Items { get; set; } = new();
}
