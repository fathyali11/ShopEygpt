using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Web.Entites.Consts;
using Web.Entites.ViewModels.CartItemVMs;
using Web.Entites.ViewModels.CartVMs;

namespace Web.DataAccess.Repositories;

public class CartRepository(ApplicationDbContext context,
    ILogger<CartRepository> _logger,
    HybridCache _hybridCache) : ICartRepository
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
            cart.TotalPrice += cartItem.TotalPrice;
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
        await RemoveCacheKeysAsync(userId, cancellationToken);
    }


    public async Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cacheKey=$"{CartCacheKeys.CartItemCount}_{userId}";
        var count = await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.CartItems.
            Where(x => x.Cart.UserId == userId)
            .CountAsync(cancellationToken), cancellationToken: cancellationToken);
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
            }).FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken),cancellationToken:cancellationToken);

        return response is not null ? response :
            new CartResponse { UserId = userId, Items = [] };
    }
    public async Task<(int,decimal)> IncreaseAsync(string userId,Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var cartItem = await _context.CartItems
            .Where(ci => ci.Id == cartItemVM.cartItemId)
            .Select(ci => new { Item = ci, Cart = ci.Cart })
            .FirstOrDefaultAsync(cancellationToken);

        if (cartItem is not null)
        {
            cartItem.Item.Count++;
            cartItem.Cart!.TotalPrice += cartItem.Item.Price;
            _logger.LogInformation("Cart item count increased to {Count}", cartItem.Item.Count);
            _logger.LogInformation("Cart total price updated to {TotalPrice}", cartItem.Cart.TotalPrice);
            await _context.SaveChangesAsync(cancellationToken);
            await RemoveCacheKeysAsync(userId, cancellationToken);
            return (cartItem.Item.Count,cartItem.Cart!.TotalPrice);
        }
        await RemoveCacheKeysAsync(userId, cancellationToken);
        return (0,0.0m);
    }
    public async Task<(int, decimal)> DecreaseAsync(string userId,Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var cartItem = await _context.CartItems
            .Select(x => new
            {
                Item = x,
                Cart = x.Cart,
            }).FirstOrDefaultAsync(x=>x.Item.Id==cartItemVM.cartItemId);
        if (cartItem is not null)
        {
            if (cartItem.Item.Count > 1)
            {
                cartItem.Item.Count--;
                cartItem.Cart!.TotalPrice -= cartItem.Item.Price;
                _logger.LogInformation("Cart item count decreased to {Count}", cartItem.Item.Count);
                _logger.LogInformation("Cart total price updated to {TotalPrice}", cartItem.Cart.TotalPrice);
                await _context.SaveChangesAsync(cancellationToken);
                await RemoveCacheKeysAsync(userId, cancellationToken);
                return (cartItem.Item.Count,cartItem.Cart!.TotalPrice);
            }
            else
            {
                _context.CartItems.Remove(cartItem.Item);
                await _context.SaveChangesAsync(cancellationToken);
                await RemoveCacheKeysAsync(userId, cancellationToken);
                return (0, 0.0m); 
            }
        }
        await RemoveCacheKeysAsync(userId, cancellationToken);
        return (0, 0.0m);
    }

    public async Task<decimal> DeleteCartItemAsync(string userId,Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var cartItem = await _context.CartItems
            .Where(ci => ci.Id == cartItemVM.cartItemId)
            .Select(ci => new { Item = ci, Cart = ci.Cart })
            .FirstOrDefaultAsync(cancellationToken);
        if (cartItem is not null)
        {
            _context.CartItems.Remove(cartItem.Item);
            cartItem.Cart!.TotalPrice -= cartItem.Item.TotalPrice;
            await _context.SaveChangesAsync(cancellationToken);
            await RemoveCacheKeysAsync(userId, cancellationToken);
            return cartItem.Cart.TotalPrice;
        }
        await RemoveCacheKeysAsync(userId, cancellationToken);
        return 0.0m;
    }

    private async Task RemoveCacheKeysAsync(string userId,CancellationToken cancellationToken = default)
    {
        await _hybridCache.RemoveAsync($"{CartCacheKeys.CartItemCount}_{userId}", cancellationToken);
        await _hybridCache.RemoveAsync($"{CartCacheKeys.CartItems}_{userId}", cancellationToken);
    }
}
