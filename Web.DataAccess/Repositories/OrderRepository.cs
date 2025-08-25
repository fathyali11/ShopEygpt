using Hangfire;

namespace Web.DataAccess.Repositories;
public class OrderRepository(ApplicationDbContext _context,
    IPaymentRepository _paymentRepository,
    HybridCache _hybridCache,
    ILogger<OrderRepository>_logger) : IOrderRepository
{
    public async Task<PaginatedList<OrderResponseVM>> GetAllOrdersAsync(FilterRequest request, CancellationToken cancellationToken=default)
    {
        var cacheKey = $"{OrderCacheKeys.AllOrders}" +
            $"_{request.SearchTerm}_{request.SortField}" +
            $"_{request.SortOrder}_{request.PageNumber}";
        var orders=await _hybridCache.GetOrCreateAsync(cacheKey,
            async _=>
            {
                var query= _context.Orders.AsNoTracking()
                .ProjectToType<OrderResponseVM>();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                    query = query.Where(o => o.UserName.Contains(request.SearchTerm));

                query = request.SortField?.ToLower() switch
                {
                    "username" => request.SortOrder == "asc" ? query.OrderBy(o => o.UserName) : query.OrderByDescending(o => o.UserName),
                    "status" => request.SortOrder == "asc" ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
                    "createdat" => request.SortOrder == "asc" ? query.OrderBy(o => o.CreatedAt) : query.OrderByDescending(o => o.CreatedAt),
                    _ => query.OrderBy(o => o.Id),
                };

                return await query.ToListAsync(cancellationToken);
            },
            tags: [OrderCacheKeys.OrdersTag]
            , cancellationToken:cancellationToken);

        var response = PaginatedList<OrderResponseVM>.Create(orders, request.PageNumber, PaginationConstants.DefaultPageSize);
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
    public async Task<OrderDetailsReponseVM> GetOrderDetailsAsync(int id,CancellationToken cancellationToken=default)
    {
        string cacheKey = $"{OrderCacheKeys.OrderDetails}_{id}";
        var orderDetails = await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.Orders
            .AsNoTracking()
            .ProjectToType<OrderDetailsReponseVM>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            , tags: [OrderCacheKeys.OrdersTag]
            , cancellationToken: cancellationToken);

        return orderDetails!;
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


    public async Task<IEnumerable<OrderProfileVM>> GetCurrentOrdersForUserAsync(string userId,CancellationToken cancellationToken=default)
    {
        var cacheKey = $"{OrderCacheKeys.CurrentOrders}_{userId}";
        var orders = await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId && o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Deleted)
            .ProjectToType<OrderProfileVM>()
            .ToListAsync(cancellationToken),
            tags: [OrderCacheKeys.OrdersTag],
            cancellationToken: cancellationToken);
        
        return orders;
    }
    private async Task RemoveCacheKeys(string userId,CancellationToken cancellationToken)
    {
        await _hybridCache.RemoveByTagAsync(OrderCacheKeys.OrdersTag, cancellationToken);
        await _hybridCache.RemoveAsync($"{CartCacheKeys.CartItemCount}_{userId}", cancellationToken);
        await _hybridCache.RemoveAsync($"{CartCacheKeys.CartItems}_{userId}", cancellationToken);
    }
}
