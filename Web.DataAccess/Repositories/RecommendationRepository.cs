namespace Web.DataAccess.Repositories;
public class RecommendationRepository(IProductRatingRepository _productRatingRepository,
    IProductRecommenderRepository _productRecommenderRepository,
    HybridCache _hybridCache):IRecommendationRepository
{
    public async Task<List<(int productId, float score)>> GetTopRecommendationsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var ratings = await _productRatingRepository.GetAllProductRatingsAsync(cancellationToken);
        if (!ratings.Any())
            return [];

        var allProducts = ratings.Select(r => r.ProductId).Distinct();
        var userProducts = ratings.Where(r => r.UserId == userId).Select(r => r.ProductId).ToHashSet();

        string cacheKey = $"{ProductCacheKeys.ProductIdsAndScore}_{userId}";

        var recommendations = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async entry =>
            {
                var result = allProducts
                    .Where(p => !userProducts.Contains(p))
                    .Select(p => (p, _productRecommenderRepository.Predict(userId, p)))
                    .OrderByDescending(x => x.Item2)
                    .Take(5)
                    .ToList();

                return await Task.FromResult(result);
            },
            tags: [$"{ProductCacheKeys.RecommendationsFullProducts}"],
            
            cancellationToken:cancellationToken
        );

        return recommendations;
    }
}
