namespace Web.Entites.IRepositories;
public interface ICartRepository
{
    Task AddToCartAsync(string userId, AddCartItemVM addCartItemVM, CancellationToken cancellationToken = default);
    Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<CartResponse> GetCartItemsAsync(string userId, CancellationToken cancellationToken = default);
    Task<(int, decimal)> IncreaseAsync(string userId, Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default);
    Task<(int, decimal)> DecreaseAsync(string userId, Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default);
    Task<decimal> DeleteCartItemAsync(string userId, Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default);

    Task RemoveCacheKeysAsync(CancellationToken cancellationToken = default);
}
