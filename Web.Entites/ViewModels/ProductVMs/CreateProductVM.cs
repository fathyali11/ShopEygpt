using Microsoft.AspNetCore.Http;

namespace Web.Entites.ViewModels.ProductVMs;

public class CreateProductVM
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(50, ErrorMessage = "Product name must be at most 50 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description must be at most 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    public int CategoryId { get; set; }
    public IFormFile? ImageFile { get; set; }
}