namespace Web.Entites.Models;
public class WishlistItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
    public int WishlistId { get; set; }
    public decimal Price { get; set; } = 0.0m;

    public Wishlist Wishlist { get; set; } = default!;
}
