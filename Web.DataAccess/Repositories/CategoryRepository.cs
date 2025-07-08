using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneOf;
using Web.Entites.ViewModels.CategoryVMs;

namespace Web.DataAccess.Repositories
{
    public class CategoryRepository(ApplicationDbContext context,
        ValidationRepository _validationRepository,
        IValidator<CreateCategoryVM> _createCategoryValidator,
        IValidator<EditCategoryVM> _editCategoryValidator) : GenericRepository<Category>(context), ICategoryRepository
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
        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await GetAllAsync();
            return categories.Adapt<IEnumerable<CategoryResponse>>();
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
        //public async Task DeleteCategoryAsync(Category category)
        //{
        //    if (!string.IsNullOrEmpty(category.ImageName))
        //    {
        //        var imagePath = Path.Combine("wwwroot", SD.ImagePathCategories, category.ImageName);
        //        if (File.Exists(imagePath))
        //        {
        //            File.Delete(imagePath);
        //        }
        //    }
        //    await RemoveAsync(category);
        //    await _context.SaveChangesAsync();
        //}
        //public async Task UpdateCategoryAsync(Category model)
        //{
        //    var category = await GetByAsync(x => x.Id == model.Id);
        //    if (category != null)
        //    {
        //        category.Name = model.Name;
        //        var hasNewImage = model.ImageCover != null;
        //        var oldImageName = category.ImageName;
        //        if (hasNewImage)
        //        {
        //            category.ImageName = await SaveImageAsync(model.ImageCover);
        //            if (!string.IsNullOrEmpty(oldImageName))
        //            {
        //                var imagePath = Path.Combine("wwwroot", SD.ImagePathCategories, oldImageName);
        //                if (File.Exists(imagePath))
        //                {
        //                    File.Delete(imagePath);
        //                }
        //            }
        //        }
        //        await _context.SaveChangesAsync();
        //    }
        //}
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
        // create a method to delete the image file if it exists
        private static void DeleteImageFile(string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) return;
            string imagePath = Path.Combine("wwwroot", SD.ImagePathCategories, imageName);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
    }
}