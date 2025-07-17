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
        Task<DiscoverProductVM> GetDiscoverProductByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<NewArrivalProductsVM> GetNewArrivalProductByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<NewArrivalProductsVM>> GetNewArrivalProductsAsync(CancellationToken cancellationToken = default);
        Task<List<BestSellingProductVM>> GetBestSellingProductsAsync(CancellationToken cancellationToken = default);
        Task<List<DiscoverProductVM>> GetDiscoverProductsAsync(CancellationToken cancellationToken = default);
        Task<List<DiscoverProductVM>> GetAllProductsSortedByAsync(string sortedBy, CancellationToken cancellationToken = default);
        Task<IEnumerable<NewArrivalProductsVM>> GetAllProductsInCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<EditProductVM?> GetProductEditByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<OneOf<List<ValidationError>, bool>> UpdateProductAsync(EditProductVM model, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(int id, CancellationToken cancellationToken = default);
    }
}