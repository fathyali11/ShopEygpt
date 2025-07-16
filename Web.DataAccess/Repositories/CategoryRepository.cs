using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Hybrid;
using OneOf;
using Web.Entites.ViewModels.CategoryVMs;

namespace Web.DataAccess.Repositories
{
    public class CategoryRepository(ApplicationDbContext context,
        ValidationRepository _validationRepository,
        IValidator<CreateCategoryVM> _createCategoryValidator,
        IValidator<EditCategoryVM> _editCategoryValidator,
        HybridCache _hybridCache) : GenericRepository<Category>(context), ICategoryRepository
    {
        private readonly ApplicationDbContext _context= context;

        public async Task<OneOf<List<ValidationError>,bool>> AddCategoryAsync(CreateCategoryVM categoryVM,CancellationToken cancellationToken=default)
        {
            var validationResult = await _validationRepository.ValidateRequest(_createCategoryValidator, categoryVM);
            if (validationResult is not null)
                return validationResult;

            var existingCategory = await GetByAsync(x => x.Name == categoryVM.Name);
            if (existingCategory != null)
                return new List<ValidationError> { new("Duplicate Category", "Category with this name already exists") };

            var category = categoryVM.Adapt<Category>();
            category.ImageName = await SaveImageAsync(categoryVM.Image);
            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var cacheKey = "AllCategories";
            var response = await _hybridCache.GetOrCreateAsync(cacheKey,
                async _ =>
                {
                    var categories = await GetAllAsync();
                    return categories.Adapt<List<CategoryResponse>>();
                });
            return response;
        }
        public async Task<IEnumerable<SelectListItem>> GetAllCategoriesSelectListAsync()
        {
            return await _context.Categories
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToListAsync();
        }
        public async Task<EditCategoryVM> GetCategoryAsync(int id)
        {
            var category = await GetByAsync(x => x.Id == id);
            if (category == null)
                return new EditCategoryVM(id, null!,null!,null!);

            return category.Adapt<EditCategoryVM>();

        }
        public async Task<OneOf<List<ValidationError>, bool>> UpdateCategoryAsync(EditCategoryVM categoryVM,CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationRepository.ValidateRequest(_editCategoryValidator, categoryVM);
            if (validationResult is not null)
                return validationResult;

            var category = await GetByAsync(x => x.Id == categoryVM.Id);
            if (category == null)
                return new List<ValidationError> { new("Not Found","Category not found") };

            var categoryImageOldName = category.ImageName;

            categoryVM.Adapt(category);
            if (categoryVM.Image != null)
            {
                DeleteImageFile(categoryImageOldName);
                category.ImageName = await SaveImageAsync(categoryVM.Image);
            }
            else
            {
                category.ImageName = categoryImageOldName;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<IEnumerable<SelectListItem>> CategorySelectListAsync()
        {
            var categories = await GetAllAsync();
            return categories.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
        }
        public async Task<OneOf<List<ValidationError>, bool>> DeleteCategoryAsync(int id)
        {
            var categoryFromDb = await GetByAsync(x => x.Id == id);
            if (categoryFromDb == null)
                return new List<ValidationError> { new("Not Found", "Category not found") };

            var existingProducts = await _context.Products.AnyAsync(x => x.CategoryId == id);
            if (existingProducts)
                return new List<ValidationError> { new("Cannot Delete", "Category cannot be deleted as it has associated products") };

            DeleteImageFile(categoryFromDb.ImageName);
            _context.Categories.Remove(categoryFromDb);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}