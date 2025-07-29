using Microsoft.Extensions.Logging;
using Web.Entites.ViewModels.CartItemVMs;

namespace Web.DataAccess.Repositories;

public class CartRepository(ApplicationDbContext context,ILogger<CartRepository> _logger) : ICartRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task AddToCartAsync(string userId, AddCartItemVM addCartItemVM, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (cart == null)
        {
            cart = new Cart { UserId = userId,TotalPrice=0.0m, CartItems = [] };
            await _context.AddAsync(cart, cancellationToken);
        }

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(x => x.ProductId == addCartItemVM.ProductId, cancellationToken);

        if (cartItem is not null)
        {
            cartItem.Count++;
            cart.TotalPrice+=addCartItemVM.Price;
        }
        else
        {
            cart.CartItems.Add(new CartItem 
            { 
                ProductId = addCartItemVM.ProductId,
                Price=addCartItemVM.Price, 
                ProductName=addCartItemVM.ProductName
                ,Count = 1, 
                ImageName = addCartItemVM.ImageName ?? string.Empty 
            });
            cart.TotalPrice+=addCartItemVM.Price;
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        return cart?.CartItems.Sum(x => x.CartId) ?? 0;
    }

    public async Task<Cart> GetCartItemsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        cart!.TotalPrice = cart.CartItems.Sum(x => x.TotalPrice);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("total price of cart: {TotalPrice}", cart.TotalPrice);
        return cart ?? new Cart
        {
            UserId = userId,
            CartItems = []
        };
    }
    public async Task<(int,decimal)> IncreaseAsync(Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(x => x.Id == cartItemVM.cartItemId, cancellationToken);
        var cart=await _context.Carts.FindAsync(cartItemVM.cartId, cancellationToken);
        if (cartItem is not null)
        {
            cartItem.Count++;
            cart!.TotalPrice += cartItem.Price;
            _logger.LogInformation("Cart item count increased to {Count}", cartItem.Count);
            _logger.LogInformation("Cart total price updated to {TotalPrice}", cart.TotalPrice);
            await _context.SaveChangesAsync(cancellationToken);
            return (cartItem.Count,cart!.TotalPrice);
        }
        return (0,0.0m);
    }
    public async Task<(int, decimal)> DecreaseAsync(Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(x => x.Id == cartItemVM.cartItemId, cancellationToken);
        var cart = await _context.Carts.FindAsync(cartItemVM.cartId, cancellationToken);
        if (cartItem is not null)
        {
            if (cartItem.Count > 1)
            {
                cartItem.Count--;
                cart!.TotalPrice -= cartItem.Price;
                _logger.LogInformation("Cart item count decreased to {Count}", cartItem.Count);
                _logger.LogInformation("Cart total price updated to {TotalPrice}", cart.TotalPrice);
                await _context.SaveChangesAsync(cancellationToken);
                return (cartItem.Count,cart!.TotalPrice);
            }
            else
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync(cancellationToken);
                return (0, 0.0m); 
            }
        }
        return (0, 0.0m);
    }

    public async Task<decimal> DeleteCartItemAsync(Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(x => x.Id == cartItemVM.cartItemId, cancellationToken);
        var cart = await _context.Carts.FindAsync(cartItemVM.cartId, cancellationToken);
        if (cartItem is not null)
        {
            _context.CartItems.Remove(cartItem);
            cart!.TotalPrice -= cartItem.TotalPrice;
            await _context.SaveChangesAsync(cancellationToken);
            return cart.TotalPrice;
        }
        return 0.0m;
    }
}
