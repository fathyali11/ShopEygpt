using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Entites.Models;
using Web.Entites.ViewModels;

namespace Web.Entites.IRepositories
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        void Update(ProductVM model);
        void AddProductVM(ProductVM Model);
        void DeleteWithImage(Product product);
    }
}
