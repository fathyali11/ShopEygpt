using Microsoft.Extensions.Caching.Hybrid;
using Web.Entites.Consts;
using Web.Entites.ViewModels.WishlistVMs;

namespace Web.DataAccess.Repositories;
public class WishlistRepository(ApplicationDbContext _context,
    HybridCache _hybridCache) : IWishlistRepository
{
    public async Task<bool> ToggelWishlistItemAsync(string userId, AddWishlistItem addWishlistItem, CancellationToken cancellationToken = default)
    {
        var wishlist = await _context.Wishlist
            .Include(x => x.WishlistItems)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if(wishlist == null)
        {
            wishlist = new Wishlist
            {
                UserId = userId,
            };
            await _context.Wishlist.AddAsync(wishlist,cancellationToken);
        }

        var wishlistItem=wishlist.WishlistItems.FirstOrDefault(x=>x.ProductId==addWishlistItem.ProductId);

        if (wishlistItem is not null)
        {
            wishlist.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync(cancellationToken);
            await _hybridCache.RemoveAsync($"{WishlistCacheKeys.WishlistItems}_{userId}");
            return false;
        }
        else
        {
            wishlist.WishlistItems.Add(new WishlistItem
            {
                ProductId = addWishlistItem.ProductId,
                Price = addWishlistItem.Price,
                ProductName = addWishlistItem.ProductName,
                ImageName = addWishlistItem.ImageName,
            });
            await _context.SaveChangesAsync(cancellationToken);
            await _hybridCache.RemoveAsync($"{WishlistCacheKeys.WishlistItems}_{userId}");
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
                .Include(x => x.WishlistItems)
                .Select(x => new WishlistResponse(
                    x.Id,
                    x.WishlistItems
                    )).FirstOrDefaultAsync(cancellationToken),cancellationToken:cancellationToken
            );


        return wishlist!;

    }

    public async Task<int> GetWishlistItemCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var wishlist = await _context.Wishlist
            .Include(x => x.WishlistItems)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if(wishlist == null) return 0;

        return wishlist.WishlistItems.Count;
    }
}
