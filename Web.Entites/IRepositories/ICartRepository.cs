using Web.Entites.Models;

namespace Web.Entites.IRepositories
{
    public interface ICartRepository:IGenericRepository<Cart>
    {
        decimal GetTotalPrice(IEnumerable<Cart> shoppingCarts);
        Task AddToCartAsync(string userId, int productId, CancellationToken cancellationToken = default);
    }
}
