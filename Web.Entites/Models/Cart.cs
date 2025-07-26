namespace Web.Entites.Models;
public class Cart
{
    public int Id {  get; set; }
    public string UserId {  get; set; }= string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public decimal TotalPrice { get; set; } = 0.0m;

    public List<CartItem> CartItems { get; set; } = [];
}
