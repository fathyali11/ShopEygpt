public record EditCategoryVM
(
    int Id,
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Category name must be between 3 and 30 characters.")]
    string Name,
    string ExistImageUrl,
    IFormFile? Image
);
