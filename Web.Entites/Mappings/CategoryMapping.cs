using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.CategoryVMs;
namespace Web.Entites.Mappings;
public class CategoryMapping
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<CreateCategoryVM, Category>.NewConfig();

        TypeAdapterConfig<Category, CategoryResponse>.NewConfig();

        TypeAdapterConfig<Category, EditCategoryVM>.NewConfig()
            .Map(dest => dest.ExistImageUrl, src => $"/Images/Categories/{src.ImageName}");




    }
}
