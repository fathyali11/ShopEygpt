using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.ProductVMs;
namespace Web.Entites.Mappings;

public class ProductMapping
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<CreateProductVM, Product>.NewConfig();

        TypeAdapterConfig<Product, ProductReponseForAdmin>.NewConfig()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty);


        TypeAdapterConfig<Product, DiscoverProductVM>.NewConfig()
            .Map(dest=>dest.CategoryName,src=>src.Category.Name);

        TypeAdapterConfig<Product, EditProductVM>.NewConfig()
        .Map(dest => dest.CategoryName, src => src.Category.Name);

        TypeAdapterConfig<EditProductVM, Product>.NewConfig()
        .Ignore(dest => dest.Category) 
        .Ignore(dest => dest.ImageName);

    }
}
