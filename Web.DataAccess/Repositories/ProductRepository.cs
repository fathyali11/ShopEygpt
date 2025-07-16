using FluentValidation;
using Mapster;
using Microsoft.Extensions.Caching.Hybrid;
using OneOf;
using Web.Entites.Models;
using Web.Entites.ViewModels.ProductVMs;

namespace Web.DataAccess.Repositories
{
    public class ProductRepository(ApplicationDbContext context,
        ValidationRepository _validationRepository,
        IValidator<CreateProductVM> _createProductValidator,
        HybridCache _hybridCache) : GenericRepository<Product>(context), IProductRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<OneOf<List<ValidationError>, bool>> AddProductAsync(CreateProductVM model,CancellationToken cancellationToken=default)
        {
            var validationResult = await _validationRepository.ValidateRequest(_createProductValidator, model);
            if (validationResult is not null)
                return validationResult;
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(x => x.Name == model.Name, cancellationToken);
            if (existingProduct is not null)
                return new List<ValidationError> { new("Name", "Product with this name already exists") };
            var product = model.Adapt<Product>();
            product.ImageName = await SaveImageAsync(model.ImageFile);
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<List<ProductReponseForAdmin>> GetAllProductsAdminAsync(CancellationToken cancellationToken = default)
        {
            var response= await _context.Products
                .Include(x => x.Category)
                .Select(x => new ProductReponseForAdmin
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    ImageName = x.ImageName,
                    CategoryName = x.Category.Name,
                    HasSale= x.IsSale,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    SoldCount=x.SoldCount,
                    TotalStock=x.TotalStock

                })
                .ToListAsync(cancellationToken);
            return response;
        }
        public async Task<EditProductVM?> GetProductEditByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .Include(x => x.Category)
                .Select(x => new EditProductVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    ImageName = x.ImageName,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name
                })
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return product is not null ? product : null;
        }
        public async Task<DiscoverProductVM> GetDiscoverProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response=await _context.Products
                .AsNoTracking()
                .Where(x=>x.Id==id)
                .ProjectToType<DiscoverProductVM>()
                .FirstOrDefaultAsync(cancellationToken);
            return response!;
        }
        public async Task<NewArrivalProductsVM> GetNewArrivalProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _context.Products
                .AsNoTracking()
                .Where(x => x.Id == id)
                .ProjectToType <NewArrivalProductsVM>()
                .FirstOrDefaultAsync(cancellationToken);
            return response!;
        }
        public async Task<List<NewArrivalProductsVM>> GetNewArrivalProductsAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = "NewArrivalProducts";
            var response = await _hybridCache.GetOrCreateAsync(cacheKey,
                async _ =>
                {
                    return await _context.Products
                            .AsNoTracking()
                            .OrderBy(x => x.CreatedAt)
                            .Take(10)
                            .ProjectToType<NewArrivalProductsVM>()
                            .ToListAsync(cancellationToken);
                },
                cancellationToken:cancellationToken
                );

            
            return response!;
        }
        public async Task<List<BestSellingProductVM>> GetBestSellingProductsAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = "BestSellingProducts";

            var response = await _hybridCache.GetOrCreateAsync(cacheKey,
                async _ => await _context.Products
                .AsNoTracking()
                .OrderBy(x => x.SoldCount)
                .Take(10)
                .ProjectToType<BestSellingProductVM>()
                .ToListAsync(cancellationToken),
                cancellationToken: cancellationToken);
            return response!;
        }
        public async Task<List<DiscoverProductVM>> GetDiscoverProductsAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = "DiscoverProducts";
            var response = await _hybridCache.GetOrCreateAsync(cacheKey,
                async _ => await _context.Products
                .AsNoTracking()
                .ProjectToType<DiscoverProductVM>()
                .ToListAsync(cancellationToken), cancellationToken: cancellationToken);
            
            return response!;
        }
        public async Task<IEnumerable<NewArrivalProductsVM>> GetAllProductsInCategoryAsync(int categoryId,CancellationToken cancellationToken = default)
        {
            var response = await _context.Products
                .Where(x=>x.CategoryId==categoryId)
                .Select(x => new NewArrivalProductsVM
                (
                    x.Id,
                    x.Name,
                    x.ImageName,
                    x.Price
                ))
                .ToListAsync(cancellationToken);
            return response!;
        }
        public async Task<OneOf<List<ValidationError>,bool>> UpdateProductAsync(EditProductVM model,CancellationToken cancellationToken=default)
        {
            var productFromDb=await _context.Products
                .FirstOrDefaultAsync(x=>x.Id==model.Id,cancellationToken);
            var oldImageName = productFromDb!.ImageName;
            var categoryId = productFromDb.CategoryId;
            model.Adapt(productFromDb);

            if (model.ImageFile is not null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(productFromDb!.ImageName))
                {
                    var oldImagePath = Path.Combine("wwwroot", SD.ImagePathProducts, productFromDb.ImageName);
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }
                productFromDb.ImageName = await SaveImageAsync(model.ImageFile);
            }
            else
            productFromDb.ImageName = oldImageName;
            if (model.CategoryId is null || model.CategoryName is null)
                productFromDb.CategoryId = categoryId;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task DeleteProductAsync(int id,CancellationToken cancellationToken=default)
        {
            var productFromDb = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            
            // Delete image if exists
            if (!string.IsNullOrEmpty(productFromDb!.ImageName))
            {
                var imagePath = Path.Combine("wwwroot", SD.ImagePathProducts, productFromDb.ImageName);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }
            _context.Products.Remove(productFromDb);
            await _context.SaveChangesAsync(cancellationToken);
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