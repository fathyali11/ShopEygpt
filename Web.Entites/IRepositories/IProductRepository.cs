using OneOf;
using Web.Entites.Models;
using Web.Entites.ViewModels;
using Web.Entites.ViewModels.ProductVMs;

namespace Web.Entites.IRepositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<OneOf<List<ValidationError>, bool>> AddProductAsync(CreateProductVM model, CancellationToken cancellationToken = default);
        Task UpdateProductAsync(EditProductVM model);
        Task DeleteProductAsync(int id);
    }
}