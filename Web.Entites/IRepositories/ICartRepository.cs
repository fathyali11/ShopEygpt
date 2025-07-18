using Web.Entites.Models;

namespace Web.Entites.IRepositories
{
    public interface ICartRepository:IGenericRepository<Cart>
    {
        Task AddToCartAsync(string userId, int productId, CancellationToken cancellationToken = default);
        Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default);
    }
}
