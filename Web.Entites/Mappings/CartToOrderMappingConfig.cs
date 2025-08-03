using Mapster;
using Web.Entites.Models;

namespace Web.Entites.Mappings;
public class CartToOrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Cart, Order>()
            .Map(dest => dest.TotalPrice, src => src.TotalPrice)
            .Map(dest => dest.OrderItems, src => src.CartItems.Adapt<List<OrderItem>>());

        config.NewConfig<CartItem, OrderItem>()
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.ProductName, src => src.ProductName)
            .Map(dest => dest.ImageName, src => src.ImageName)
            .Map(dest => dest.UnitPrice, src => src.Price)
            .Map(dest => dest.Quantity, src => src.Count);
    }
}
