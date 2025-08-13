
namespace Web.DataAccess.Repositories;
public class ProductRatingRepository(ApplicationDbContext _context) : IProductRatingRepository
{
    public async Task AddOrUpdateRatingAsync(string userId, int productId, int rating, CancellationToken cancellationToken = default)
    {
        var existing = await _context.ProductRatings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId, cancellationToken);

        if (existing is null)
        {
            await _context.ProductRatings.AddAsync(new ProductRating
            {
                UserId = userId,
                ProductId = productId,
                Rating = rating
            }, cancellationToken);
        }
        else
        {
            await _context.ProductRatings
                .Where(r => r.ProductId == productId && r.UserId == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(r => r.Rating, rating)
                    .SetProperty(r => r.UpdatedAt, DateTime.UtcNow),
                    cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateRatingsForPurchaseAsync(string userId, List<int> productIds, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId) || productIds == null || productIds.Count == 0)
            return false;

        var affectedRows = await _context.ProductRatings
            .Where(r => r.UserId == userId && productIds.Contains(r.ProductId))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.Rating, RatingNumbers.BuyItem)
                .SetProperty(r => r.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        return affectedRows > 0;
    }

}
