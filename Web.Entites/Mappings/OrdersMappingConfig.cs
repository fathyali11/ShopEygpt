namespace Web.Entites.Mappings;

public class OrdersMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderDetailsReponseVM>()
           .Map(dest => dest.UserName, src => src.User.UserName)
           .Map(dest => dest.OrderItems, src => src.OrderItems.Adapt<List<OrderItemDetailsVM>>());

        config.NewConfig<OrderItem, OrderItemDetailsVM>();
    }
}