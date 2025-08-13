namespace Web.Entites.IRepositories;
public interface ICategoryRepository
{
    Task<OneOf<List<ValidationError>, bool>> AddCategoryAsync(CreateCategoryVM categoryVM, CancellationToken cancellationToken = default);
    Task<EditCategoryVM?> GetCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<OneOf<List<ValidationError>, bool>> UpdateCategoryAsync(EditCategoryVM categoryVM, CancellationToken cancellationToken = default);
    Task<OneOf<List<ValidationError>, bool>> DeleteCategoryAsync(int id);


    Task<PaginatedList<Category>> GetAllCategoriesAsync(int pageNumber, CancellationToken cancellationToken = default);
    Task<OneOf<PaginatedList<CategoryInHomeVM>, List<CategoryInHomeVM>>> GetAllCategoriesInHomeAsync(bool isAll, int pageNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<SelectListItem>> GetAllCategoriesSelectListAsync(CancellationToken cancellationToken=default);
    
}