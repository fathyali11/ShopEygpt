using Hangfire;

namespace Web.DataAccess.Repositories;
public class ProductRepository(ApplicationDbContext context,
    GeneralRepository _generalRepository,
    IValidator<CreateProductVM> _createProductValidator,
    HybridCache _hybridCache,
    IWishlistRepository _wishlistRepository,
    IRecommendationRepository _recommendationRepository) : IProductRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<OneOf<List<ValidationError>, bool>> AddProductAsync(CreateProductVM model,CancellationToken cancellationToken=default)
    {
        var validationResult = await _generalRepository.ValidateRequest(_createProductValidator, model);
        if (validationResult is not null)
            return validationResult;
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(x => x.Name == model.Name, cancellationToken);
        if (existingProduct is not null)
            return new List<ValidationError> { new("Name", "Product with this name already exists") };
        var product = model.Adapt<Product>();
        product.ImageName = await _generalRepository.SaveImageAsync(model.ImageFile, SD.ImagePathProducts);
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await RemoveKeys(cancellationToken);
        return true;
    }
    public async Task<OneOf<List<ValidationError>, bool>> UpdateProductAsync(EditProductVM model, CancellationToken cancellationToken = default)
    {
        var productFromDb = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);
        var oldImageName = productFromDb!.ImageName;
        var categoryId = productFromDb.CategoryId;
        model.Adapt(productFromDb);

        if (model.ImageFile is not null)
        {
            _generalRepository.DeleteImage(oldImageName, SD.ImagePathProducts);
            productFromDb.ImageName = await _generalRepository.SaveImageAsync(model.ImageFile, SD.ImagePathProducts);
        }
        else
            productFromDb.ImageName = oldImageName;
        if (model.CategoryId is null || model.CategoryName is null)
            productFromDb.CategoryId = categoryId;
        await _context.SaveChangesAsync(cancellationToken);
        await RemoveKeys(cancellationToken);
        return true;
    }
    public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var productFromDb = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (productFromDb is null)
            return false;
        _generalRepository.DeleteImage(productFromDb!.ImageName, SD.ImagePathProducts);
        _context.Products.Remove(productFromDb);
        var numberOfChanges=await _context.SaveChangesAsync(cancellationToken);
        await RemoveKeys(cancellationToken);

        return numberOfChanges>0?true:false;
    }
    public async Task<PaginatedList<ProductReponseForAdmin>> GetAllProductsAdminAsync(int pageNumber,CancellationToken cancellationToken = default)
    {
        var cacheKey = ProductCacheKeys.AllProductsAdmin;
        var products= await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.Products
            .ProjectToType<ProductReponseForAdmin>()
            .ToListAsync(cancellationToken),
            cancellationToken: cancellationToken);

        return PaginatedList<ProductReponseForAdmin>.Create(products, pageNumber, PaginationConstants.DefaultPageSize);

    }
    public async Task<EditProductVM?> GetProductEditByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .Where(x => x.Id == id)
            .ProjectToType<EditProductVM>()
            .FirstOrDefaultAsync(cancellationToken);
        return product is not null ? product : null;
    }
    public async Task<ProductReponseForAdmin?> GetProductDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .Where(x => x.Id == id)
            .ProjectToType<ProductReponseForAdmin>()
            .FirstOrDefaultAsync(cancellationToken);
        return product is not null ? product : null;
    }



    public async Task<DiscoverProductVM> GetDiscoverProductByIdAsync(string userId,int id, CancellationToken cancellationToken = default)
    {
        var response=await _context.Products
            .AsNoTracking()
            .Where(x=>x.Id==id)
            .ProjectToType<DiscoverProductVM>()
            .FirstOrDefaultAsync(cancellationToken);
        if(!string.IsNullOrEmpty(userId))
        {
            BackgroundJob.Enqueue<IProductRatingRepository>(repo =>
            repo.AddOrUpdateRatingAsync(userId, id, RatingNumbers.ViewItem, cancellationToken));
        }
        

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
    public async Task<List<NewArrivalProductsVM>> GetNewArrivalProductsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cacheKey =ProductCacheKeys.NewArrivalProducts;
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

        if (!string.IsNullOrEmpty(userId))
        {
            var wishlist = await _wishlistRepository.GetWishlistItems(userId, cancellationToken);
            if (wishlist is not null)
            {
                response = response.Select(x => new NewArrivalProductsVM(
                x.Id,
                x.Name,
                x.ImageName,
                x.Price,
                wishlist.Items.Any(w => w.Id == x.Id)))
                .ToList();
            }


        }


        return response!;
    }
    public async Task<List<BestSellingProductVM>> GetBestSellingProductsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = ProductCacheKeys.BestSellingProducts;

        var response = await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.Products
            .AsNoTracking()
            .OrderBy(x => x.SoldCount)
            .Take(10)
            .ProjectToType<BestSellingProductVM>()
            .ToListAsync(cancellationToken),
            cancellationToken: cancellationToken);

        if (!string.IsNullOrEmpty(userId))
        {
            var wishlist = await _wishlistRepository.GetWishlistItems(userId, cancellationToken);
            if (wishlist is not null)
            {
                response = response.Select(x => new BestSellingProductVM(
                x.Id,
                x.Name,
                x.ImageName,
                x.Price,
                wishlist.Items.Any(w => w.ProductId == x.Id)))
                .ToList();
            }


        }

        return response!;
    }
    public async Task<List<DiscoverProductVM>> GetDiscoverProductsAsync(string userId,CancellationToken cancellationToken = default)
    {
        var cacheKey = ProductCacheKeys.DiscoverProducts;
        var response = await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.Products
            .AsNoTracking()
            .ProjectToType<DiscoverProductVM>()
            .ToListAsync(cancellationToken), cancellationToken: cancellationToken);
        if (!string.IsNullOrEmpty(userId))
        {
            var wishlist = await _wishlistRepository.GetWishlistItems(userId, cancellationToken);
            if (wishlist is not null)
            {
                response = response.Select(x => new DiscoverProductVM(
                x.Id,
                x.Name,
                x.Description,
                x.ImageName,
                x.CategoryName,
                x.Price,
                wishlist.Items.Any(w => w.ProductId == x.Id)))
                .ToList();
            }
        }

        return response!;
    }
    public async Task<PaginatedList<DiscoverProductVM>> GetAllProductsSortedByAsync(string userId,string sortedBy,int pageNumber, CancellationToken cancellationToken = default)
    {
        var cacheKey = ProductCacheKeys.AllProductsSortedBy;

        var response = await _hybridCache.GetOrCreateAsync(cacheKey, async _ =>
        {
            IQueryable<Product> query = _context.Products.AsNoTracking();
            query = sortedBy switch
            {
                "SoldCount" => query.OrderByDescending(p => p.SoldCount),
                "CreatedAt" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Id)
            };

            return await query
                .ProjectToType<DiscoverProductVM>()
                .ToListAsync(cancellationToken);
        }, cancellationToken: cancellationToken);


        if(!string.IsNullOrEmpty(userId))
        {
            var wishlist = await _wishlistRepository.GetWishlistItems(userId, cancellationToken);
            if(wishlist is not null)
            {
                response = response.Select(x => new DiscoverProductVM(
                x.Id,
                x.Name,
                x.Description,
                x.ImageName,
                x.CategoryName,
                x.Price,
                wishlist.Items.Any(w => w.ProductId == x.Id)))
                .ToList();
            }
            
            
        }



        return PaginatedList<DiscoverProductVM>.Create(response, pageNumber, 6);
    }
    public async Task<List<DiscoverProductVM>> GetRecommendationsProducts(string userId,CancellationToken cancellationToken=default)
    {
        var recommendationsProductIdsAndScores=await _recommendationRepository.GetTopRecommendationsAsync(userId, cancellationToken);
        string cacheKey = $"{ProductCacheKeys.RecommendationsFullProducts}_{userId}";

        var products=await _hybridCache.GetOrCreateAsync(cacheKey,
           async _=> await _context.Products
            .AsNoTracking()
            .Where(x => recommendationsProductIdsAndScores.Select(x => x.productId).Contains(x.Id))
            .ProjectToType<DiscoverProductVM>()
            .ToListAsync(cancellationToken)
            , tags: [$"{ProductCacheKeys.Recommendations}"]
            ,cancellationToken:cancellationToken);

        return products;

    }
    public async Task<IEnumerable<DiscoverProductVM>> GetAllProductsInCategoryAsync(int categoryId,CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{ProductCacheKeys.AllProductsInCategory}{categoryId}";
        return await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ => await _context.Products
            .Where(x => x.CategoryId == categoryId)
            .ProjectToType<DiscoverProductVM>()
            .ToListAsync(cancellationToken),
            cancellationToken: cancellationToken);
    }
   
    private async Task RemoveKeys(CancellationToken cancellationToken = default)
    {
        await _hybridCache.RemoveAsync(ProductCacheKeys.AllProductsInCategory, cancellationToken);
        await _hybridCache.RemoveAsync(ProductCacheKeys.NewArrivalProducts, cancellationToken);
        await _hybridCache.RemoveAsync(ProductCacheKeys.BestSellingProducts, cancellationToken);
        await _hybridCache.RemoveAsync(ProductCacheKeys.DiscoverProducts, cancellationToken);
        await _hybridCache.RemoveAsync(ProductCacheKeys.AllProductsAdmin, cancellationToken);
        await _hybridCache.RemoveAsync(ProductCacheKeys.AllProductsSortedBy, cancellationToken);
        await _hybridCache.RemoveByTagAsync([$"{ProductCacheKeys.Recommendations}"], cancellationToken);
    }
    
}