using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Entites.Models;

namespace Web.Entites.IRepositories
{
    public interface IShoppingCartRepository:IGenericRepository<Cart>
    {
        decimal GetTotalPrice(IEnumerable<Cart> shoppingCarts);
    }
}
