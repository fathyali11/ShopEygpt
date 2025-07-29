using Web.Entites.Models;

namespace Web.Entites.IRepositories
{
    public interface IOrderHeaderReposittory
    {
        void Update(OrderHeader orderHeader);
        void UpdateStatus(int id, string OrderStatus,string PaymentStatus);
    }
}
