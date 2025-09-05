namespace Web.Entites.IRepositories;
public interface IRecommendationRepository
{
    Task<List<(int productId, float score)>> GetTopRecommendationsAsync(string userId, CancellationToken cancellationToken = default);
}
