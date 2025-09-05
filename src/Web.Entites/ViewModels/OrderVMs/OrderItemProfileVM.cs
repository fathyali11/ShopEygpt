namespace Web.Entites.ViewModels.OrderVMs;

public class OrderItemProfileVM
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
