
namespace Web.DataAccess.Repositories;
public class OrderRepository : IOrderRepository
{
    public Task<Order> CreateOrderAsync(string userId, string PaymentIntentId, string sessionId)
    {
        throw new NotImplementedException();
    }
}
