using Microsoft.AspNetCore.Http;

namespace Web.Entites.ViewModels.CategoryVMs;
public record CreateCategoryVM(string Name,IFormFile Image);
