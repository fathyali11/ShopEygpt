using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Hybrid;
using OneOf;
using System.Threading;
using Web.Entites.Consts;
using Web.Entites.ViewModels.CategoryVMs;

namespace Web.DataAccess.Repositories
{
    public class CategoryRepository(ApplicationDbContext context,
        GeneralRepository _generalRepository,
        IValidator<CreateCategoryVM> _createCategoryValidator,
        IValidator<EditCategoryVM> _editCategoryValidator,
        HybridCache _hybridCache) : GenericRepository<Category>(context), ICategoryRepository
    {
        private readonly ApplicationDbContext _context= context;

        public async Task<OneOf<List<ValidationError>,bool>> AddCategoryAsync(CreateCategoryVM categoryVM,CancellationToken cancellationToken=default)
        {
            var validationResult = await _generalRepository.ValidateRequest(_createCategoryValidator, categoryVM);
            if (validationResult is not null)
                return validationResult;

            var existingCategory = await GetByAsync(x => x.Name == categoryVM.Name);
            if (existingCategory != null)
                return new List<ValidationError> { new("Duplicate Category", "Category with this name already exists") };

            var category = categoryVM.Adapt<Category>();
            category.ImageName = await _generalRepository.SaveImageAsync(categoryVM.Image, SD.ImagePathCategories);
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

            var category = await GetByAsync(x => x.Id == categoryVM.Id);
            if (category == null)
                return new List<ValidationError> { new("Not Found", "Category not found") };

            var categoryImageOldName = category.ImageName;

            categoryVM.Adapt(category);
            if (categoryVM.Image != null)
            {
                _generalRepository.DeleteImage(categoryImageOldName, SD.ImagePathCategories);
                category.ImageName = await _generalRepository.SaveImageAsync(categoryVM.Image, SD.ImagePathCategories);
            }
            else
            {
                category.ImageName = categoryImageOldName;
            }
            await _context.SaveChangesAsync(cancellationToken);
            await RemoveKeys(cancellationToken);
            await RemoveProductCacheKeys(category.Id,cancellationToken);
            return true;
        }
        public async Task<OneOf<List<ValidationError>, bool>> DeleteCategoryAsync(int id)
        {
            var categoryFromDb = await GetByAsync(x => x.Id == id);
            if (categoryFromDb == null)
                return new List<ValidationError> { new("Not Found", "Category not found") };

            var existingProducts = await _context.Products.AnyAsync(x => x.CategoryId == id);
            if (existingProducts)
                return new List<ValidationError> { new("Cannot Delete", "Category cannot be deleted as it has associated products") };

            _generalRepository.DeleteImage(categoryFromDb.ImageName, SD.ImagePathCategories);
            _context.Categories.Remove(categoryFromDb);
            await _context.SaveChangesAsync();
            await RemoveKeys();
            return true;
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync(CancellationToken cancellationToken=default)
        {
            var cacheKey = CategoryCacheKeys.AllCategories;
            var response = await _hybridCache.GetOrCreateAsync(cacheKey,
                async _ =>
                {
                    var categories = await GetAllAsync();
                    return categories.Adapt<List<CategoryResponse>>();
                },cancellationToken:cancellationToken);
            return response;
        }
        public async Task<List<CategoryInHomeVM>> GetAllCategoriesInHomeAsync(bool isAll,CancellationToken cancellationToken=default)
        {
            var cacheKey = isAll?CategoryCacheKeys.AllCategoriesInHome: CategoryCacheKeys.LimitedCategoriesInHome;
            
            return await _hybridCache.GetOrCreateAsync(cacheKey,
                async _ =>
                {
                    var query =_context.Categories
                    .AsNoTracking()
                    .ProjectToType<CategoryInHomeVM>();

                    if (isAll)
                        query = query.Take(4);

                    return await query.ToListAsync(cancellationToken);

                }
                ,
                cancellationToken: cancellationToken
                );

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
                cancellationToken:cancellationToken);
        }
        


        private async Task RemoveKeys(CancellationToken cancellationToken=default)
        {
            await _hybridCache.RemoveAsync(CategoryCacheKeys.AllCategories, cancellationToken);
            await _hybridCache.RemoveAsync(CategoryCacheKeys.AllCategoriesInHome, cancellationToken);
            await _hybridCache.RemoveAsync(CategoryCacheKeys.AllCategoriesSelectList, cancellationToken);
            await _hybridCache.RemoveAsync(CategoryCacheKeys.LimitedCategoriesInHome, cancellationToken);
        }

        private async Task RemoveProductCacheKeys(int categoryId,CancellationToken cancellationToken = default)
        {
            await _hybridCache.RemoveAsync($"{ProductCacheKeys.AllProductsInCategory}{categoryId}", cancellationToken);
            await _hybridCache.RemoveAsync(ProductCacheKeys.NewArrivalProducts, cancellationToken);
            await _hybridCache.RemoveAsync(ProductCacheKeys.BestSellingProducts, cancellationToken);
            await _hybridCache.RemoveAsync(ProductCacheKeys.DiscoverProducts, cancellationToken);
            await _hybridCache.RemoveAsync(ProductCacheKeys.AllProductsAdmin, cancellationToken);
            await _hybridCache.RemoveAsync(ProductCacheKeys.AllProductsSortedBy, cancellationToken);
        }
    }
}