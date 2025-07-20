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
            var cart=await _context.Carts
                .Include(x=>x.CartItems)
                .FirstOrDefaultAsync(x=>x.UserId==userId,cancellationToken);

            if(cart==null)
            {
                cart = new Cart { UserId = userId, CartItems = [] };
                await _context.AddAsync(cart, cancellationToken);
            }

            var cartItem=await _context.CartItems
                .FirstOrDefaultAsync(x=>x.ProductId==productId,cancellationToken);

            if (cartItem is not null)
                cartItem.Count++;
            else
            {
                cart.CartItems.Add(new CartItem { ProductId=productId,Count=1 });
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
            return cart?.CartItems.Sum(x=>x.CartId) ?? 0;
        }
    }
}
