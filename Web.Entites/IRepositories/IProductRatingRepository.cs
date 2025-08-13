namespace Web.Entites.IRepositories;
public interface IProductRatingRepository
{
    Task<bool> AddRatingAsync(string userId,int productId,int rating,CancellationToken cancellationToken=default);
}
