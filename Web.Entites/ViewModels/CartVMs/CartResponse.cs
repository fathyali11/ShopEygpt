using Web.Entites.Models;

namespace Web.Entites.ViewModels.CartVMs;
public class CartResponse
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; } = 0;
    public List<CartItem> Items { get; set; } = [];
}