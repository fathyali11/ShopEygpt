namespace Web.Entites.IRepositories;
public interface IProductRatingRepository
{
    Task AddOrUpdateRatingAsync(string userId, int productId, int rating, CancellationToken cancellationToken = default);
    Task<bool> UpdateRatingsForPurchaseAsync(string userId, List<int> productIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductRating>> GetAllProductRatingsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductRating>> GetAllProductRatingsForUserAsync(string userId, CancellationToken cancellationToken = default);
}
