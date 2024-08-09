using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.DataAccess.Repositories
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public decimal GetTotalPrice(IEnumerable<ShoppingCart> shoppingCarts)
        {
            decimal totalPrice = 0;
            foreach(var item in shoppingCarts)
            {
                totalPrice += (item.Product.Price * item.Count);
            }
            return totalPrice;
        }
    }
}
