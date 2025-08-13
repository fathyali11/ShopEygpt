namespace Web.Entites.Models;
public class CartItem
{
    public int Id {  get; set; }
    public int ProductId {  get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
    public int CartId {  get; set; }
    public int Count {  get; set; }
    public decimal Price { get; set; } = 0.0m;
    public decimal TotalPrice => Price * Count;

    public Cart Cart { get; set; } = default!;
}
