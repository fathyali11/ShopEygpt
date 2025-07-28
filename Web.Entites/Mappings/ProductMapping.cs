using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.ProductVMs;
namespace Web.Entites.Mappings;

public class ProductMapping:IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateProductVM, Product>();

        config.NewConfig<Product, ProductReponseForAdmin>()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty);


        config.NewConfig<Product, DiscoverProductVM>()
            .Map(dest=>dest.CategoryName,src=>src.Category.Name);

        config.NewConfig<Product, EditProductVM>()
        .Map(dest => dest.CategoryName, src => src.Category.Name);

        config.NewConfig<EditProductVM, Product>()
        .Ignore(dest => dest.Category) 
        .Ignore(dest => dest.ImageName);

    }
}
