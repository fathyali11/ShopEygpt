using Web.Entites.Models;

namespace Web.Entites.IRepositories;
public interface IOrderRepository
{
    Task<Order?> CreateOrderAsync(string userId, string PaymentIntentId, string sessionId, CancellationToken cancellationToken = default);
}
