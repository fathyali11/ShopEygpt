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
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<EditCategoryVM> GetCategoryAsync(int id);
        Task<OneOf<List<ValidationError>, bool>> UpdateCategoryAsync(EditCategoryVM categoryVM, CancellationToken cancellationToken = default);
        //Task DeleteCategoryAsync(Category category);
        //Task UpdateCategoryAsync(Category model);
        Task<IEnumerable<SelectListItem>> CategorySelectListAsync();
    }
}