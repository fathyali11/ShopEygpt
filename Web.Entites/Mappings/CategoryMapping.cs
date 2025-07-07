using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.CategoryVMs;
namespace Web.Entites.Mappings;
public class CategoryMapping
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<CreateCategoryVM, Category>.NewConfig()
            .Map(dest => dest.ImageName, src => src.Image.FileName);

        TypeAdapterConfig<Category, CategoryResponse>.NewConfig();
    }
}
