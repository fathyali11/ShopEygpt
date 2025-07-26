using Web.Entites.Models;
using Web.Entites.ViewModels.CartItemVMs;

namespace Web.Entites.IRepositories
{
    public interface ICartRepository:IGenericRepository<Cart>
    {
        Task AddToCartAsync(string userId, AddCartItemVM addCartItemVM, CancellationToken cancellationToken = default);
        Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default);
        Task<Cart> GetCartItemsAsync(string userId, CancellationToken cancellationToken = default);
        Task<(int, decimal)> IncreaseAsync(Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default);
        Task<(int, decimal)> DecreaseAsync(Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default);
        Task<decimal> DeleteCartItemAsync(Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default);
    }
}
