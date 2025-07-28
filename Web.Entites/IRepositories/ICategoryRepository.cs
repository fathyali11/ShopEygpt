using Microsoft.AspNetCore.Mvc.Rendering;
using OneOf;
using System.Threading.Tasks;
using Web.Entites.Models;
using Web.Entites.ViewModels;
using Web.Entites.ViewModels.CategoryVMs;

namespace Web.Entites.IRepositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<OneOf<List<ValidationError>, bool>> AddCategoryAsync(CreateCategoryVM categoryVM, CancellationToken cancellationToken = default);
        Task<List<CategoryResponse>> GetAllCategoriesAsync(CancellationToken cancellationToken=default);
        Task<List<CategoryInHomeVM>> GetAllCategoriesInHomeAsync(bool isAll, CancellationToken cancellationToken = default);
        Task<IEnumerable<SelectListItem>> GetAllCategoriesSelectListAsync(CancellationToken cancellationToken=default);
        Task<EditCategoryVM?> GetCategoryAsync(int id, CancellationToken cancellationToken = default);
        Task<OneOf<List<ValidationError>, bool>> UpdateCategoryAsync(EditCategoryVM categoryVM, CancellationToken cancellationToken = default);
        Task<OneOf<List<ValidationError>, bool>> DeleteCategoryAsync(int id);
    }
}