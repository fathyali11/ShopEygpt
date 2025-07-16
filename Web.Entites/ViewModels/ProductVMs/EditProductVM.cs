using Microsoft.AspNetCore.Http;

namespace Web.Entites.ViewModels.ProductVMs;

public class EditProductVM
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string? ImageName { get; set; }
    public string ?CategoryName { get; set; } 
    public int ?CategoryId { get; set; }
    public bool HasSale { get; set; }
    public IFormFile? ImageFile { get; set; } 
    public int TotalStock { get; set; }
}