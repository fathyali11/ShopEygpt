
using Mapster;
using Microsoft.Extensions.Caching.Hybrid;
using Web.Entites.Consts;
using Web.Entites.ViewModels.OrderVMs;

namespace Web.DataAccess.Repositories;
public class OrderRepository(ApplicationDbContext _context,
    HybridCache _hybridCache) : IOrderRepository
{
    public async Task<PaginatedList<OrderResponseVM>> GetAllOrdersAsync(int pageNumber,CancellationToken cancellationToken=default)
    {
        var cacheKey = OrderCacheKeys.AllOrders;

        var orders=await _hybridCache.GetOrCreateAsync(cacheKey,
            async _=> await _context.Orders
            .Select(x => new OrderResponseVM(
                x.Id,
                x.UserId,
                x.Status
                ))
            .ToListAsync(cancellationToken),cancellationToken:cancellationToken);

        var response = PaginatedList<OrderResponseVM>.Create(orders, pageNumber, PaginationConstants.DefaultPageSize);
        return response;
    }
    public async Task<Order?> CreateOrderAsync(string userId, string PaymentIntentId, string sessionId,CancellationToken cancellationToken=default)
    {
        var cart = await _context.Carts.
            Include(x=>x.CartItems).
            FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        if (cart == null)
            return null;

        var order=cart.Adapt<Order>();
        order.UserId = userId;
        order.PaymentIntentId=PaymentIntentId;
        order.StripeSessionId=sessionId;
        order.Status =OrderStatus.Paid;

        await _context.Orders.AddAsync(order,cancellationToken);
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync(cancellationToken);
        await RemoveCacheKeys(cancellationToken);
        return order;

    }
    public async Task<Order> GetOrderDetailsAsync(int id,CancellationToken cancellationToken=default)
    {
        var order=await _context.Orders
            .Include(x=>x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return order!;
    }

    private async Task RemoveCacheKeys(CancellationToken cancellationToken)
    {
        await _hybridCache.RemoveAsync(OrderCacheKeys.AllOrders, cancellationToken);
    }
}
