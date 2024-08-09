
using Web.Entites.Models;

namespace Web.Entites.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
