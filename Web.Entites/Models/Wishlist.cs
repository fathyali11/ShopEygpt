namespace Web.Entites.Models;

public class Wishlist
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public List<WishlistItem> WishlistItems { get; set; } = [];
}

