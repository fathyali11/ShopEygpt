namespace Web.DataAccess.Repositories;
public class PaymentRepository(ApplicationDbContext _context,
    IOptions<StripeSettings> options,
    IHttpContextAccessor _httpContextAccessor):IPaymentRepository
{
    private readonly StripeSettings _options=options.Value;
    public async Task<string?> CreateCheckoutSessionAsync(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
            return null;

        var lineItems = cart.CartItems.Select(item => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmountDecimal = item.Price * 100,
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.ProductName
                }
            },
            Quantity = item.Count
        }).ToList();

        var request = _httpContextAccessor.HttpContext!.Request;
        var successUrl = $"{request.Scheme}://{request.Host}/Orders/Success?session_id={{CHECKOUT_SESSION_ID}}";
        var cancelUrl = $"{request.Scheme}://{request.Host}/Orders/Failed";

        var options = new SessionCreateOptions
        {
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }

    public async Task<bool> RefundPaymentAsync(string paymentIntentId,CancellationToken cancellationToken=default)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            var service = new RefundService();
            Refund refund = await service.CreateAsync(options, cancellationToken: cancellationToken);
            return string.Equals(refund.Status, RefundStatus.succeeded,StringComparison.OrdinalIgnoreCase);

        }
        catch (Exception)
        {
            return false;
        }
    }
}
