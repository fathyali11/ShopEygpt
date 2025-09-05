namespace Web.Entites.ViewModels.ProductVMs;
public class EditProductVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(50, ErrorMessage = "Product name must be at most 50 characters.")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Description must be at most 500 characters.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    public string? ImageName { get; set; }

    public string? CategoryName { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Stock quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative integer.")]
    public int TotalStock { get; set; }

    public int SoldCount { get; set; }

    public IFormFile? ImageFile { get; set; }
}
