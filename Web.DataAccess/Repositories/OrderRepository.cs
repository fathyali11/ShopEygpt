using Hangfire;

namespace Web.DataAccess.Repositories;
public class OrderRepository(ApplicationDbContext _context,
    IPaymentRepository _paymentRepository,
    HybridCache _hybridCache,
    ILogger<OrderRepository>_logger) : IOrderRepository
{
    public async Task<PaginatedList<OrderResponseVM>> GetAllOrdersAsync(int pageNumber,CancellationToken cancellationToken=default)
    {
        var cacheKey = OrderCacheKeys.AllOrders;

        var orders=await _hybridCache.GetOrCreateAsync(cacheKey,
            async _=> await _context.Orders
            .Select(x => new OrderResponseVM(
                x.Id,
                x.UserId,
                x.User.UserName!,
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
        var productIds=cart.CartItems.Select(x=>x.ProductId).ToList();
        var order=cart.Adapt<Order>();
        order.UserId = userId;
        order.PaymentIntentId=PaymentIntentId;
        order.StripeSessionId=sessionId;
        order.Status =OrderStatus.Paid;
        await _context.Orders.AddAsync(order,cancellationToken);
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync(cancellationToken);
        await RemoveCacheKeys(userId, cancellationToken);

        BackgroundJob.Enqueue<IProductRatingRepository>(repo =>
        repo.UpdateRatingsForPurchaseAsync(userId, productIds, cancellationToken));

        return order;

    }
    public async Task<Order> GetOrderDetailsAsync(int id,CancellationToken cancellationToken=default)
    {
        var order=await _context.Orders
            .Include(x=>x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return order!;
    }

    public async Task<bool> CancelOrderAsync(string userId, int id, CancellationToken cancellationToken=default)
    {
        _logger.LogInformation("we will get order with id = {id}", id);
        var order = await _context.Orders.FindAsync(id);

        if(order is null || string.Equals(order.Status,OrderStatus.Cancelled,StringComparison.OrdinalIgnoreCase))
            return false;

        _logger.LogInformation($"Cancel Order: {order.Status}");

        var isRefunded = await _paymentRepository.RefundPaymentAsync(order.PaymentIntentId, cancellationToken);

        if(!isRefunded) return false;

        _logger.LogInformation("refund money to customer");
        order.Status=OrderStatus.Cancelled;
        await _context.SaveChangesAsync(cancellationToken);
        await RemoveCacheKeys(userId, cancellationToken);

        return true;
    }
    public async Task<bool> DeleteOrderAsync(string userId, int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("we will get order with id = {id}", id);

        var order = await _context.Orders.FindAsync(id);


        if (order is null || string.Equals(order.Status, OrderStatus.Deleted, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("order with id = {id} is already deleted",id);
            return false;
        }

        var isRefunded = true;

        if (string.Equals(order.Status,OrderStatus.Paid,StringComparison.OrdinalIgnoreCase))
            isRefunded = await _paymentRepository.RefundPaymentAsync(order.PaymentIntentId, cancellationToken);


        if (!isRefunded)
            return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
        await RemoveCacheKeys(userId,cancellationToken);

        return true;
    }
    private async Task RemoveCacheKeys(string userId,CancellationToken cancellationToken)
    {
        await _hybridCache.RemoveAsync(OrderCacheKeys.AllOrders, cancellationToken);
        await _hybridCache.RemoveAsync($"{CartCacheKeys.CartItemCount}_{userId}", cancellationToken);
        await _hybridCache.RemoveAsync($"{CartCacheKeys.CartItems}_{userId}", cancellationToken);
    }
}
