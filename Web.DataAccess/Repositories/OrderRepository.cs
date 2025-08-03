
using Mapster;
using Web.Entites.Consts;

namespace Web.DataAccess.Repositories;
public class OrderRepository(ApplicationDbContext _context) : IOrderRepository
{
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
        return order;

    }
}
