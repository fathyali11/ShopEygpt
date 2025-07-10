using OneOf;
using Web.Entites.Models;
using Web.Entites.ViewModels;
using Web.Entites.ViewModels.ProductVMs;

namespace Web.Entites.IRepositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<OneOf<List<ValidationError>, bool>> AddProductAsync(CreateProductVM model, CancellationToken cancellationToken = default);
        Task<List<ProductReponseForAdmin>> GetAllProductsAdminAsync(CancellationToken cancellationToken = default);
        Task<EditProductVM?> GetProductEditByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<OneOf<List<ValidationError>, bool>> UpdateProductAsync(EditProductVM model, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(int id, CancellationToken cancellationToken = default);
    }
}