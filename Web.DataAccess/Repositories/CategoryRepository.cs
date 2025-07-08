using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneOf;
using Web.Entites.ViewModels.CategoryVMs;

namespace Web.DataAccess.Repositories
{
    public class CategoryRepository(ApplicationDbContext context,
        ValidationRepository _validationRepository,
        IValidator<CreateCategoryVM> _createCategoryValidator) : GenericRepository<Category>(context), ICategoryRepository
    {
        private readonly ApplicationDbContext _context= context;

        public async Task<OneOf<List<ValidationError>,bool>> AddCategoryAsync(CreateCategoryVM categoryVM,CancellationToken cancellationToken=default)
        {
            var validationResult = await _validationRepository.ValidateRequest(_createCategoryValidator, categoryVM);
            if (validationResult is not null)
                return validationResult;

            var category = categoryVM.Adapt<Category>();
            category.ImageName = await SaveImageAsync(categoryVM.Image);
            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<IEnumerable<SelectListItem>> CategorySelectListAsync()
        {
            var categories = await GetAllAsync();
            return categories.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
        }
        public async Task DeleteCategoryAsync(Category category)
        {
            if (!string.IsNullOrEmpty(category.ImageName))
            {
                var imagePath = Path.Combine("wwwroot", SD.ImagePathCategories, category.ImageName);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            await RemoveAsync(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCategoryAsync(Category model)
        {
            var category = await GetByAsync(x => x.Id == model.Id);
            if (category != null)
            {
                category.Name = model.Name;
                var hasNewImage = model.ImageCover != null;
                var oldImageName = category.ImageName;
                if (hasNewImage)
                {
                    category.ImageName = await SaveImageAsync(model.ImageCover);
                    if (!string.IsNullOrEmpty(oldImageName))
                    {
                        var imagePath = Path.Combine("wwwroot", SD.ImagePathCategories, oldImageName);
                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
        private static async Task<string> SaveImageAsync(IFormFile cover)
        {
            string imageName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";
            string imagePath = Path.Combine("wwwroot", SD.ImagePathCategories);
            string path = Path.Combine(imagePath, imageName);
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            using var stream = new FileStream(path, FileMode.Create);
            await cover.CopyToAsync(stream);
            return imageName;
        }
    }
}