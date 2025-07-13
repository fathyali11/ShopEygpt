namespace Web.Entites.Models;
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId {  get; set; }
    public Category Category { get; set; }=default!;
    public string ImageName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsSale { get; set; } = false;
    public int TotalStock { get; set; } = 0;
    public int SoldCount { get; set; } = 0;
}
