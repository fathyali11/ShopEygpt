namespace Web.Entites.IRepositories;
public interface IPaymentRepository
{
    Task<string?> CreateCheckoutSessionAsync(string userId);
}
