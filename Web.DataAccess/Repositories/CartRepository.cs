using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.DataAccess.Repositories
{
    public class CartRepository(ApplicationDbContext context) : GenericRepository<Cart>(context), ICartRepository
    {
        private readonly ApplicationDbContext _context = context;
        public async Task AddToCartAsync(string userId, int productId, CancellationToken cancellationToken = default)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, CartItems = [] };
                await _context.AddAsync(cart, cancellationToken);
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);

            if (cartItem is not null)
                cartItem.Count++;
            else
            {
                var imageName = await _context.Products
                    .Where(x => x.Id == productId)
                    .Select(x => x.ImageName)
                    .FirstOrDefaultAsync(cancellationToken);
                cart.CartItems.Add(new CartItem { ProductId = productId, Count = 1, ImageName = imageName ?? string.Empty });
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
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
            return cart ?? new Cart
            {
                UserId = userId,
                CartItems = []
            };
        }
        public async Task<int> IncreaseAsync(int cartItemId, CancellationToken cancellationToken = default)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.Id == cartItemId, cancellationToken);
            if (cartItem is not null)
            {
                cartItem.Count++;
                await _context.SaveChangesAsync(cancellationToken);
                return cartItem.Count;
            }
            return 0;
        }
        public async Task<int> DecreaseAsync(int cartItemId, CancellationToken cancellationToken = default)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.Id == cartItemId, cancellationToken);
            if (cartItem is not null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    await _context.SaveChangesAsync(cancellationToken);
                    return cartItem.Count;
                }
                else
                {
                    _context.CartItems.Remove(cartItem);
                    await _context.SaveChangesAsync(cancellationToken);
                    return 0;
                }
            }
            return 0;
        }

        public async Task DeleteCartItemAsync(int cartItemId, CancellationToken cancellationToken = default)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.Id == cartItemId, cancellationToken);
            if (cartItem is not null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
