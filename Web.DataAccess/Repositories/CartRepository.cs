using Hangfire;

namespace Web.DataAccess.Repositories;
public class CartRepository(ApplicationDbContext context,
    ILogger<CartRepository> _logger,
    HybridCache _hybridCache,
    IBackgroundJobsRepository _backgroundJobsRepository) : ICartRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task AddToCartAsync(string userId, AddCartItemVM addCartItemVM, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId,
                TotalPrice = 0.0m,
                CartItems = []
            };
            await _context.Carts.AddAsync(cart, cancellationToken);
           
            await _context.SaveChangesAsync(cancellationToken);
        }

        var cartItem = await _context.CartItems
            .Where(x => x.Cart.Id == cart.Id && x.ProductId == addCartItemVM.ProductId)
            .FirstOrDefaultAsync(cancellationToken);

        if (cartItem is not null)
        {
            cartItem.Count += addCartItemVM.Count;
            cart.TotalPrice += cartItem.Price;
        }
        else
        {
            var newItem = new CartItem
            {
                ProductId = addCartItemVM.ProductId,
                ProductName = addCartItemVM.ProductName,
                ImageName = addCartItemVM.ImageName ?? string.Empty,
                Price = addCartItemVM.Price,
                Count = addCartItemVM.Count
            };
            cart.CartItems.Add(newItem);
            cart.TotalPrice += newItem.TotalPrice;
        }
        await _context.SaveChangesAsync(cancellationToken);
        BackgroundJob.Enqueue<IProductRatingRepository>(repo =>
        repo.AddOrUpdateRatingAsync(userId, addCartItemVM.ProductId, RatingNumbers.AddToCart, cancellationToken));

        await RemoveCacheKeysAsync( cancellationToken);
    }


    public async Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cacheKey=$"{CartCacheKeys.CartItemCount}_{userId}";
        var count = await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.CartItems.
            Where(x => x.Cart.UserId == userId)
            .CountAsync(cancellationToken),
            tags: [CartCacheKeys.CartsTag],
            cancellationToken: cancellationToken);
        return count;
    }

    public async Task<CartResponse> GetCartItemsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CartCacheKeys.CartItems}_{userId}";
        var response=await _hybridCache.GetOrCreateAsync(cacheKey,
            async _=> await _context.Carts
            .Select(x => new CartResponse
            {
                Id= x.Id,
                UserId= x.UserId,
                TotalPrice= x.TotalPrice,
                Items= x.CartItems.ToList()
            }).FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken),
            tags: [CartCacheKeys.CartsTag],
            cancellationToken:cancellationToken);

        return response is not null ? response :
            new CartResponse { UserId = userId, Items = [] };
    }
    public async Task<Delete_Increase_DecreaseCartItemResponse> IncreaseAsync(string userId,Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var result = await _context.CartItems
            .Select(ci => new { Item = ci, Cart = ci.Cart })
            .FirstOrDefaultAsync(x => x.Item.ProductId == cartItemVM.cartItemId && x.Cart.Id == cartItemVM.cartId);

        if (result is not null)
        {
            result.Item.Count++;
            result.Cart!.TotalPrice += result.Item.Price;
            _logger.LogInformation("Cart item count increased to {Count}", result.Item.Count);
            _logger.LogInformation("Cart total price updated to {TotalPrice}", result.Cart.TotalPrice);
            await _context.SaveChangesAsync(cancellationToken);
            await RemoveCacheKeysAsync(cancellationToken);
            return new Delete_Increase_DecreaseCartItemResponse(result.Item.Count, result.Cart!.TotalPrice);
        }
        await RemoveCacheKeysAsync(cancellationToken);
        return new Delete_Increase_DecreaseCartItemResponse(0, 0.0m);
    }
    public async Task<Delete_Increase_DecreaseCartItemResponse> DecreaseAsync(string userId,Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var result = await _context.CartItems
            .Select(x => new
            {
                Item = x,
                Cart = x.Cart,
            }).FirstOrDefaultAsync(x=>x.Item.ProductId==cartItemVM.cartItemId && x.Cart.Id == cartItemVM.cartId);
        if (result is not null)
        {
            if (result.Item.Count > 1)
            {
                result.Item.Count--;
                result.Cart!.TotalPrice -= result.Item.Price;
                _logger.LogInformation("Cart item count decreased to {Count}", result.Item.Count);
                _logger.LogInformation("Cart total price updated to {TotalPrice}", result.Cart.TotalPrice);
                await _context.SaveChangesAsync(cancellationToken);
                await RemoveCacheKeysAsync( cancellationToken);
                return new Delete_Increase_DecreaseCartItemResponse(result.Item.Count,result.Cart!.TotalPrice);
            }
            else
            {
                _context.CartItems.Remove(result.Item);
                result.Cart!.TotalPrice -= result.Item.TotalPrice;
                await _context.SaveChangesAsync(cancellationToken);
                await RemoveCacheKeysAsync( cancellationToken);
                _backgroundJobsRepository.Enqueue<IProductRatingRepository>(repo =>
                repo.AddOrUpdateRatingAsync(userId, result.Item.ProductId, RatingNumbers.RemoveFromCart, cancellationToken));

                return new Delete_Increase_DecreaseCartItemResponse(0, result.Cart.TotalPrice); 
            }
        }
        await RemoveCacheKeysAsync( cancellationToken);
        // default value as window will be refreshed and cart total price will be fetched again
        return new Delete_Increase_DecreaseCartItemResponse(0, 0.0m);
    }

    public async Task<decimal> DeleteCartItemAndReturnCartTotalPriceAsync(string userId,Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var result = await _context.CartItems
            .Select(ci => new { Item = ci, Cart = ci.Cart })
            .FirstOrDefaultAsync(x => x.Item.ProductId == cartItemVM.cartItemId && x.Cart.Id == cartItemVM.cartId); ;
        if (result is not null)
        {
            _context.CartItems.Remove(result.Item);
            result.Cart!.TotalPrice -= result.Item.TotalPrice;
            await _context.SaveChangesAsync(cancellationToken);
            await RemoveCacheKeysAsync(cancellationToken);
            _backgroundJobsRepository.Enqueue<IProductRatingRepository>(repo =>
              repo.AddOrUpdateRatingAsync(userId, result.Item.ProductId, RatingNumbers.RemoveFromCart, cancellationToken));


            return result.Cart.TotalPrice;
        }
        await RemoveCacheKeysAsync(cancellationToken);
        _backgroundJobsRepository.Enqueue<IProductRatingRepository>(repo =>
              repo.AddOrUpdateRatingAsync(userId, result!.Item.ProductId, RatingNumbers.RemoveFromCart, cancellationToken));
        // default value as window will be refreshed and cart total price will be fetched again
        return 0.0m;
    }

    public async Task ClearCartAsync(string userId, int cartId, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId, cancellationToken);

        if (cart is not null)
        {
            cart.TotalPrice = 0.0m;
            await _context.CartItems
                .Where(ci => ci.CartId == cartId && ci.Cart.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await RemoveCacheKeysAsync(cancellationToken);
        }
    }
    public async Task RemoveCacheKeysAsync(CancellationToken cancellationToken = default)
    {
        await _hybridCache.RemoveByTagAsync(CartCacheKeys.CartsTag, cancellationToken);
    }
}
