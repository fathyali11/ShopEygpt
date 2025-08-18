namespace Web.DataAccess.Repositories;
public class RecommendationRepository(IProductRatingRepository _productRatingRepository,
    IProductRecommenderRepository _productRecommenderRepository,
    HybridCache _hybridCache):IRecommendationRepository
{
    public async Task<List<(int productId, float score)>> GetTopRecommendationsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var allProducts = await _productRatingRepository.GetAllProductIdsForProductRatingsAsync(cancellationToken);
        if (!allProducts.Any())
            return [];

        var userProducts = await _productRatingRepository.GetAllProductIdsForProductRatingsForUserAsync(userId,cancellationToken);

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
            tags: [$"{ProductCacheKeys.Recommendations}"],
            
            cancellationToken:cancellationToken
        );

        return recommendations;
    }
}
