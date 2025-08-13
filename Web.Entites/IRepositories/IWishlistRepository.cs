namespace Web.Entites.IRepositories;
public interface IWishlistRepository
{
    Task<bool> ToggelWishlistItemAsync(string userId, AddWishlistItem addWishlistItem, CancellationToken cancellationToken = default);
    Task<WishlistResponse> GetWishlistItems(string userId, CancellationToken cancellationToken = default);
    Task<int> GetWishlistItemCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> DeleteWishlistItemAsync(string userId, DeleteWishlistItem deleteWishlistItem, CancellationToken cancellationToken = default);
}
