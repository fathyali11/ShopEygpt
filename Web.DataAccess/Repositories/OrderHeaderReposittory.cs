using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.DataAccess.Repositories
{
    public class OrderHeaderReposittory : GenericRepository<OrderHeader>, IOrderHeaderReposittory
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderReposittory(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }
        public void UpdateStatus(int id, string OrderStatus, string PaymentStatus)
        {
            var OrderHeaderFromDB=_context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if(OrderHeaderFromDB != null)
            {
                if (OrderStatus != null)
                    OrderHeaderFromDB.OrderStatus = OrderStatus;
                if (PaymentStatus != null)
                    OrderHeaderFromDB.PaymentStatus = PaymentStatus;
            }
        }
    }
}
