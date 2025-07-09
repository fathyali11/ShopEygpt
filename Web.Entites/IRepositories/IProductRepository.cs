using Web.Entites.Models;
using Web.Entites.ViewModels.ProductVMs;

namespace Web.Entites.IRepositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task UpdateProductAsync(EditProductVM model);
        Task AddProductAsync(CreateProductVM model);
        Task DeleteProductAsync(int id);
    }
}