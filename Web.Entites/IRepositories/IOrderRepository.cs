using Web.Entites.Models;
using Web.Entites.ViewModels.OrderVMs;

namespace Web.Entites.IRepositories;
public interface IOrderRepository
{
    Task<PaginatedList<OrderResponseVM>> GetAllOrdersAsync(int pageNumber, CancellationToken cancellationToken = default);
    Task<Order?> CreateOrderAsync(string userId, string PaymentIntentId, string sessionId, CancellationToken cancellationToken = default);
    Task<Order> GetOrderDetailsAsync(int id, CancellationToken cancellationToken = default);
}
