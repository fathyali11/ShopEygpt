using Hangfire;

namespace Web.DataAccess.Repositories;
public class WishlistRepository(ApplicationDbContext _context,
    HybridCache _hybridCache,
    IBackgroundJobsRepository _backgroundJobsRepository) : IWishlistRepository
{
    public async Task<bool> ToggelWishlistItemAsync(string userId, AddWishlistItem addWishlistItem, CancellationToken cancellationToken = default)
    {
        var wishlist = await _context.Wishlist
         .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (wishlist == null)
        {
            wishlist = new Wishlist { UserId = userId };
            await _context.Wishlist.AddAsync(wishlist, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var wishlistItem = await _context.WishlistItems
            .FirstOrDefaultAsync(x => x.WishlistId == wishlist.Id && x.ProductId == addWishlistItem.ProductId, cancellationToken);

        if (wishlistItem is not null)
        {
            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync(cancellationToken);
            await RemoveCacheKeys(cancellationToken);
            _backgroundJobsRepository.Enqueue<IProductRatingRepository>(repo =>
                    repo.AddOrUpdateRatingAsync(userId, addWishlistItem.ProductId, RatingNumbers.RemoveFromWishlist, CancellationToken.None));

            return false;
        }
        else
        {
            await _context.WishlistItems.AddAsync(new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = addWishlistItem.ProductId,
                Price = addWishlistItem.Price,
                ProductName = addWishlistItem.ProductName,
                ImageName = addWishlistItem.ImageName,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await RemoveCacheKeys(cancellationToken);
            _backgroundJobsRepository.Enqueue<IProductRatingRepository>(repo =>
                    repo.AddOrUpdateRatingAsync(userId, addWishlistItem.ProductId, RatingNumbers.AddToWishlist, CancellationToken.None));
            return true;
        }

    }
    public async Task<WishlistResponse> GetWishlistItems(string userId,CancellationToken cancellationToken=default)
    {
        var cacheKey=$"{WishlistCacheKeys.WishlistItems}_{userId}";

        var wishlist = await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ =>
            await _context.Wishlist
                .Where(x => x.UserId == userId)
                .Select(x => new WishlistResponse(
                    x.Id,
                    x.WishlistItems.Select(wl=>new WishlistItem
                    {
                        ProductId= wl.ProductId,
                        ProductName = wl.ProductName,
                        ImageName = wl.ImageName,
                        WishlistId= x.Id,
                        Price = wl.Price
                    }).ToList()
                        
                    )).FirstOrDefaultAsync(cancellationToken),
                    tags: [WishlistCacheKeys.WishlistsTag]
                    , cancellationToken:cancellationToken
            );

        return wishlist is not null ? wishlist : new WishlistResponse(0, []);

    }
    public async Task<int> GetWishlistItemCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{WishlistCacheKeys.WishlistItemCount}_{userId}";
        return await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.WishlistItems
                .Where(wi => wi.Wishlist.UserId == userId)
                .CountAsync(cancellationToken),
            tags: [WishlistCacheKeys.WishlistsTag],
            cancellationToken: cancellationToken);
    }
    public async Task<int> DeleteWishlistItemAsync(string userId,DeleteWishlistItem deleteWishlistItem, CancellationToken cancellationToken = default)
    {
        var wishlistItemFromDb = await _context.WishlistItems
            .FirstOrDefaultAsync(x => x.ProductId == deleteWishlistItem.ProductId && x.WishlistId == deleteWishlistItem.WishlistId, cancellationToken);
        if (wishlistItemFromDb == null)
            return -1;
        _context.WishlistItems.Remove(wishlistItemFromDb);
        var result = await _context.SaveChangesAsync(cancellationToken);
            

        if (result == 0)
            return -1;

        await RemoveCacheKeys(cancellationToken);
        _backgroundJobsRepository.Enqueue<IProductRatingRepository>(repo =>
        repo.AddOrUpdateRatingAsync(userId, deleteWishlistItem.ProductId, RatingNumbers.RemoveFromWishlist, CancellationToken.None));

        return await GetWishlistItemCountAsync (userId, cancellationToken);
    }
    
    public async Task ClearWishlistAsync(int wishlistId,string userId, CancellationToken cancellationToken = default)
    {
        await _context.WishlistItems
            .Where(x =>x.Wishlist.UserId == userId&&x.WishlistId==wishlistId)
            .ExecuteDeleteAsync(cancellationToken);
        await RemoveCacheKeys(cancellationToken);
    }
    public async Task RemoveCacheKeys(CancellationToken cancellationToken=default)
    {
        await _hybridCache.RemoveByTagAsync(WishlistCacheKeys.WishlistsTag,cancellationToken);
    }

}
