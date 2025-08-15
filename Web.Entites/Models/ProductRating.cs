namespace Web.Entites.Models;
public class ProductRating
{
    public string UserId { get; set; }=string.Empty;
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
