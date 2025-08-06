namespace Web.Entites.IRepositories;
public interface IPaymentRepository
{
    Task<string?> CreateCheckoutSessionAsync(string userId);
    Task<bool> RefundPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
}
