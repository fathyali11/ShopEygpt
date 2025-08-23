using MailKit.Search;

namespace Web.DataAccess.Repositories;
public class CategoryRepository(ApplicationDbContext context,
    GeneralRepository _generalRepository,
    IValidator<CreateCategoryVM> _createCategoryValidator,
    IValidator<EditCategoryVM> _editCategoryValidator,
    HybridCache _hybridCache,
    CloudinaryRepository _cloudinaryRepository,
    IProductRepository _productRepository) :ICategoryRepository
{
    private readonly ApplicationDbContext _context= context;

    public async Task<OneOf<List<ValidationError>,bool>> AddCategoryAsync(CreateCategoryVM categoryVM,CancellationToken cancellationToken=default)
    {
        var validationResult = await _generalRepository.ValidateRequest(_createCategoryValidator, categoryVM);
        if (validationResult is not null)
            return validationResult;

        var existingCategory = await _context.Categories.FirstOrDefaultAsync(x=>x.Name == categoryVM.Name);
        if (existingCategory != null)
            return new List<ValidationError> { new("Duplicate Category", "Category with this name already exists") };

        var category = categoryVM.Adapt<Category>();
        var imageUrl= await _cloudinaryRepository.UploadImageAsync(categoryVM.Image);
        if(imageUrl is null)
            return new List<ValidationError> { new("Server Error", "Internal server error in saving image") };
        category.ImageName = imageUrl;
        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await RemoveKeys(cancellationToken);
        return true;
    }
    public async Task<EditCategoryVM?> GetCategoryAsync(int id,CancellationToken cancellationToken=default)
    {
        var response=await _context.Categories
            .Where(c => c.Id == id)
            .ProjectToType<EditCategoryVM>()
            .FirstOrDefaultAsync(cancellationToken);
        return response is not null ? response : null;

    }
    public async Task<OneOf<List<ValidationError>, bool>> UpdateCategoryAsync(EditCategoryVM categoryVM, CancellationToken cancellationToken = default)
    {
        var validationResult = await _generalRepository.ValidateRequest(_editCategoryValidator, categoryVM);
        if (validationResult is not null)
            return validationResult;

        var category = await _context.Categories.FindAsync(categoryVM.Id);
        if (category == null)
            return new List<ValidationError> { new("Not Found", "Category not found") };

        var categoryImageOldName = category.ImageName;

        categoryVM.Adapt(category);
        if (categoryVM.Image != null)
        {
            var imageUrl=await _cloudinaryRepository.UpdateImageAsync(categoryImageOldName, categoryVM.Image);
            if(imageUrl is null)
                return new List<ValidationError> { new("Server Error", "Internal server error in saving image") };
            category.ImageName=imageUrl;
        }
        else
        {
            category.ImageName = categoryImageOldName;
        }
        category.UpdatedAt= DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await RemoveKeys(cancellationToken);
        return true;
    }
    public async Task<OneOf<List<ValidationError>, bool>> DeleteCategoryAsync(int id)
    {
        var categoryFromDb = await _context.Categories.FindAsync(id);
        if (categoryFromDb == null)
            return new List<ValidationError> { new("Not Found", "Category not found") };

        var existingProducts = await _context.Products.AnyAsync(x => x.CategoryId == id);
        if (existingProducts)
            return new List<ValidationError> { new("Cannot Delete", "Category cannot be deleted as it has associated products") };

        var isDeleted = await _cloudinaryRepository.DeleteImageAsync(categoryFromDb.ImageName);
        if(!isDeleted)
            return new List<ValidationError> { new("Server Error", "Internal server error in saving image") };

        _context.Categories.Remove(categoryFromDb);
        await _context.SaveChangesAsync();
        await RemoveKeys();
        return true;
    }

    public async Task<PaginatedList<Category>> GetAllCategoriesAsync(FilterRequest request, CancellationToken cancellationToken=default)
    {
        var cacheKey = $"{CategoryCacheKeys.AllCategories}" +
            $"_{request.SearchTerm}_{request.SortField}" +
            $"_{request.SortOrder}_{request.PageNumber}";

        var response = await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ =>
            {
                var query = _context.Categories.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                    query = query
                    .Where(c => c.Name.Contains(request.SearchTerm));

                query = request.SortField?.ToLower() switch
                {
                    "name" => request.SortOrder == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
                    "createdat" => request.SortOrder == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                    "updatedat" => request.SortOrder == "asc" ? query.OrderBy(c => c.UpdatedAt) : query.OrderByDescending(c => c.UpdatedAt),
                    _ => query.OrderBy(c => c.Id),
                };

                return await query.ToListAsync(cancellationToken);

            }
            ,
            tags: [CategoryCacheKeys.CategoriesTag]
            , cancellationToken:cancellationToken);
        return  PaginatedList<Category>.Create(response,request.PageNumber,PaginationConstants.DefaultPageSize);
    }
    public async Task<OneOf<PaginatedList<CategoryInHomeVM>,List<CategoryInHomeVM>>> GetAllCategoriesInHomeAsync(bool isAll,int pageNumber,CancellationToken cancellationToken=default)
    {
        var cacheKey = isAll?CategoryCacheKeys.AllCategoriesInHome: CategoryCacheKeys.LimitedCategoriesInHome;
        
        var response= await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ =>
            {
                var query =_context.Categories
                .AsNoTracking()
                .ProjectToType<CategoryInHomeVM>();

                if (!isAll)
                    query = query.Take(4);

                return await query.ToListAsync(cancellationToken);

            },
            tags: [CategoryCacheKeys.CategoriesTag],
            cancellationToken: cancellationToken
            );

        return isAll ?
            PaginatedList<CategoryInHomeVM>.Create(response, pageNumber, 8) :
            response;

    }
    public async Task<IEnumerable<SelectListItem>> GetAllCategoriesSelectListAsync(CancellationToken cancellationToken=default)
    {
        var cacheKey=CategoryCacheKeys.AllCategoriesSelectList;
        return await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.Categories
            .Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
            .ToListAsync(cancellationToken),
            tags: [CategoryCacheKeys.CategoriesTag],
            cancellationToken:cancellationToken);
    }
    


    private async Task RemoveKeys(CancellationToken cancellationToken=default)
    {
        await _hybridCache.RemoveByTagAsync(CategoryCacheKeys.CategoriesTag, cancellationToken);
        await _productRepository.RemoveKeys(cancellationToken);
    }

}