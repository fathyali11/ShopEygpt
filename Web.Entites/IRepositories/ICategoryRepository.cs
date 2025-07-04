using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Entites.Models;

namespace Web.Entites.IRepositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task AddWithImageAsync(Category category);
        Task DeleteWithImageAsync(Category category);
        Task UpdateAsync(Category model);
        Task<IEnumerable<SelectListItem>> CategorySelectListAsync();
    }
}