using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Entites.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartId {  get; set; }
        public int ProductId {  get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public string UserId {  get; set; }
        [ForeignKey(nameof(UserId))]
        [ValidateNever]
        public ApplicationUser User { get; set; }
        public int Count {  get; set; }
    }
}
