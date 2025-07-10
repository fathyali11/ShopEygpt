using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.ProductVMs;
namespace Web.Entites.Mappings;

public class ProductMapping
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<CreateProductVM, Product>.NewConfig();

        //TypeAdapterConfig<Product, ProductResponse>.NewConfig();

        TypeAdapterConfig<EditProductVM, Product>.NewConfig()
            .Map(dest => dest.IsSale, src => src.HasSale);

    }
}
