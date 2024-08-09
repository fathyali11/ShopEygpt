using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Web.Entites.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderHeaderId {  get; set; }
        [ValidateNever]
        [ForeignKey(nameof(OrderHeaderId))]
        public OrderHeader OrderHeader { get; set; }
        public int ProductId {  get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        // for products
        public int Count {  get; set; }
        public decimal Price {  get; set; }
    }
}
