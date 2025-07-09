using FluentValidation;
using Mapster;
using OneOf;
using Web.Entites.Models;
using Web.Entites.ViewModels.ProductVMs;

namespace Web.DataAccess.Repositories
{
    public class ProductRepository(ApplicationDbContext context,
        ValidationRepository _validationRepository,
        IValidator<CreateProductVM> _createProductValidator) : GenericRepository<Product>(context), IProductRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<OneOf<List<ValidationError>, bool>> AddProductAsync(CreateProductVM model,CancellationToken cancellationToken=default)
        {
            var validationResult = await _validationRepository.ValidateRequest(_createProductValidator, model);
            if (validationResult is not null)
                return validationResult;
            var product = model.Adapt<Product>();
            product.ImageName = await SaveImageAsync(model.ImageFile);
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task UpdateProductAsync(EditProductVM model)
        {
            var productDB = await _context.Products.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (productDB == null)
                throw new InvalidOperationException("Product not found.");

            if (model.ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(productDB.ImageName))
                {
                    var oldImagePath = Path.Combine("wwwroot", SD.ImagePathProducts, productDB.ImageName);
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }
                productDB.ImageName = await SaveImageAsync(model.ImageFile);
            }

            productDB.Name = model.Name;
            productDB.Description = model.Description;
            productDB.Price = model.Price;
            productDB.CategoryId = model.CategoryId;

            _context.Products.Update(productDB);
            // SaveChangesAsync should be called by UnitOfWork, not here
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
                throw new InvalidOperationException("Product not found.");

            // Delete image if exists
            if (!string.IsNullOrEmpty(product.ImageName))
            {
                var imagePath = Path.Combine("wwwroot", SD.ImagePathProducts, product.ImageName);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            _context.Products.Remove(product);
            // SaveChangesAsync should be called by UnitOfWork, not here
        }

        private static async Task<string> SaveImageAsync(IFormFile file)
        {
            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var imagesPath = Path.Combine("wwwroot", SD.ImagePathProducts);
            var path = Path.Combine(imagesPath, imageName);

            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return imageName;
        }
    }
}