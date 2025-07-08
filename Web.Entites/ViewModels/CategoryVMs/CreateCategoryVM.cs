using Microsoft.AspNetCore.Http;

public record CreateCategoryVM
(
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Category name must be between 3 and 30 characters.")]
    string Name ,

    [Required(ErrorMessage = "Image is required.")]
    IFormFile Image 
);
