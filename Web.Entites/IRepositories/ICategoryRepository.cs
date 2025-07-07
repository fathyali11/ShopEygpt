using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Entites.Models;
using Web.Entites.ViewModels.CategoryVMs;

namespace Web.Entites.IRepositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task AddCategoryAsync(CreateCategoryVM categoryVM);
        Task DeleteCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category model);
        Task<IEnumerable<SelectListItem>> CategorySelectListAsync();
    }
}