using Web.Entites.Models;

namespace Web.Entites.IRepositories
{
    public interface ICartRepository:IGenericRepository<Cart>
    {
        Task AddToCartAsync(string userId, int productId, CancellationToken cancellationToken = default);
        Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default);
        Task<Cart> GetCartItemsAsync(string userId, CancellationToken cancellationToken = default);
        Task<int> IncreaseAsync(int cartItemId, CancellationToken cancellationToken = default);
        Task<int> DecreaseAsync(int cartItemId, CancellationToken cancellationToken = default);
    }
}
