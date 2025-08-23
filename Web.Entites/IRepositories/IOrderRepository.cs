namespace Web.Entites.IRepositories;
public interface IOrderRepository
{
    Task<PaginatedList<OrderResponseVM>> GetAllOrdersAsync(FilterRequest request, CancellationToken cancellationToken = default);
    Task<Order?> CreateOrderAsync(string userId, string PaymentIntentId, string sessionId, CancellationToken cancellationToken = default);
    Task<OrderDetailsReponseVM> GetOrderDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CancelOrderAsync(string userId, int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteOrderAsync(string userId, int id, CancellationToken cancellationToken = default);
}
