namespace Web.Entites.ViewModels.ProductVMs;
public class ProductReponseForAdmin
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string ImageName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public bool HasSale { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
