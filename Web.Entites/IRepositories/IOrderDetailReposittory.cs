
using Web.Entites.Models;

namespace Web.Entites.IRepositories
{
    public interface IOrderDetailReposittory:IGenericRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
