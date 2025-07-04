using Microsoft.AspNetCore.Mvc.Rendering;
namespace Web.DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> CategorySelectListAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
        }

        public async Task AddWithImageAsync(Category category)
        {
            category.ImageName = await SaveImageAsync(category.ImageCover);
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWithImageAsync(Category category)
        {
            var imagePath = Path.Combine("wwwroot", SD.ImagePathCategories, category.ImageName);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category model)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);
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
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<string> SaveImageAsync(IFormFile cover)
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