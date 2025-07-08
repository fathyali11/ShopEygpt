using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.CategoryVMs;
namespace Web.Entites.Mappings;
public class CategoryMapping
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<CreateOrEditCategoryVM, Category>.NewConfig();

        TypeAdapterConfig<Category, CategoryResponse>.NewConfig();
    }
}
