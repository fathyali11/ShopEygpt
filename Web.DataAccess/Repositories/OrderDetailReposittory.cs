﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Web.DataAccess.Repositories
{
    public class OrderDetailReposittory : GenericRepository<OrderDetail>, IOrderDetailReposittory
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailReposittory(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }
    }
}
