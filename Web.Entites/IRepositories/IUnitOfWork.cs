using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Entites.IRepositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IOrderHeaderReposittory OrderHeaderReposittory { get; }
        IOrderDetailReposittory OrderDetailReposittory { get; }
        IApplicaionUserRepository ApplicaionUserRepository { get; }
        int Save();
    }
}
