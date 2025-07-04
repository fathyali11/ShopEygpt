using Microsoft.AspNetCore.Http;

namespace Web.Entites.ViewModels;
public class CategoryVM
{
    public string Name { get; set; } = string.Empty;
    public IFormFile? ImageCover { get; set; }
}
