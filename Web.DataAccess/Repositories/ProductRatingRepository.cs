
namespace Web.DataAccess.Repositories;
public class ProductRatingRepository(ApplicationDbContext _context) : IProductRatingRepository
{
    public async Task<bool> AddRatingAsync(string userId, int productId, int rating, CancellationToken cancellationToken = default)
    {
        if (rating < 0 || rating > 5)
            return false;

        if(userId is null)
            return false;

        await _context.ProductRatings.AddAsync(new ProductRating
        {
            UserId = userId,
            Rating = rating,
            ProductId = productId
        }, cancellationToken);
        var added=await _context.SaveChangesAsync(cancellationToken);
        if(added==0)
            return false;
        return true;
    }
}
