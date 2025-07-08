using Microsoft.AspNetCore.Mvc.Rendering;
using OneOf;
using Web.Entites.Models;
using Web.Entites.ViewModels;
using Web.Entites.ViewModels.CategoryVMs;

namespace Web.Entites.IRepositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<OneOf<List<ValidationError>, bool>> AddCategoryAsync(CreateCategoryVM categoryVM, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        //Task DeleteCategoryAsync(Category category);
        //Task UpdateCategoryAsync(Category model);
        Task<IEnumerable<SelectListItem>> CategorySelectListAsync();
    }
}