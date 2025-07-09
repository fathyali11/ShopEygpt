using Microsoft.AspNetCore.Http;

namespace Web.Entites.ViewModels.ProductVMs;

public record EditProductVM(
    int Id,

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(50, ErrorMessage = "Product name must be at most 50 characters.")]
    string Name,

    [StringLength(500, ErrorMessage = "Description must be at most 500 characters.")]
    string Description,

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0.")]
    decimal Price,

    [Required(ErrorMessage = "Category is required.")]
    int CategoryId,

    string ImageName,

    IFormFile? ImageFile
);
