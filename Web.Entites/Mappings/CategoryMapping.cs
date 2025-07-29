using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.CategoryVMs;
namespace Web.Entites.Mappings;
public class CategoryMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateCategoryVM, Category>();

        config.NewConfig<Category, CategoryResponse>();

        config.NewConfig<Category, EditCategoryVM>()
              .Map(dest => dest.ExistImageUrl,src => src.ImageName);
    }
}
