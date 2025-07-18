
using Web.Entites.Models;

namespace Web.Entites.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<Cart> ShoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
