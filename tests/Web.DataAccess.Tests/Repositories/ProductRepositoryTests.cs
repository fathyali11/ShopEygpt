using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using Web.DataAccess.Data;
using Web.DataAccess.Tests;
using Web.Entites.Consts;
using Web.Entites.IRepositories;
using Web.Entites.Models;
using Web.Entites.ViewModels;
using Web.Entites.ViewModels.ProductVMs;
using Xunit;

namespace Web.DataAccess.Repositories.Tests;

public class ProductRepositoryTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private ProductRepository CreateRepository(
        ApplicationDbContext context,
        IGeneralRepository? generalRepo = null,
        IValidator<CreateProductVM>? createValidator = null,
        HybridCache? cache = null,
        IWishlistRepository? wishlistRepo = null,
        ICartRepository? cartRepo = null,
        IRecommendationRepository? recRepo = null,
        ICloudinaryRepository? cloudinaryRepo = null,
        IBackgroundJobsRepository? backgroundJobsRepository=null)
    {
        return new ProductRepository(
            context,
            generalRepo ?? new Mock<IGeneralRepository>().Object,
            createValidator ?? new Mock<IValidator<CreateProductVM>>().Object,
            cache ?? new FakeHybridCache(),
            wishlistRepo ?? new Mock<IWishlistRepository>().Object,
            cartRepo ?? new Mock<ICartRepository>().Object,
            recRepo ?? new Mock<IRecommendationRepository>().Object,
            cloudinaryRepo ?? new Mock<ICloudinaryRepository>().Object,
            backgroundJobsRepository ?? new Mock<IBackgroundJobsRepository>().Object
        );
    }

    [Fact]
    public async Task AddProductAsync_WhenValidationFails_ReturnsValidationError()
    {
        using var context = CreateContext();
        var generalRepo = new Mock<IGeneralRepository>();
        generalRepo.Setup(x => x.ValidateRequest(It.IsAny<IValidator<CreateProductVM>>(), It.IsAny<CreateProductVM>()))
            .ReturnsAsync(new List<ValidationError> { new("Name", "Invalid") });

        var repo = CreateRepository(context, generalRepo.Object);

        var result = await repo.AddProductAsync(new CreateProductVM(string.Empty, string.Empty, default, default, default, new Mock<IFormFile>().Object));

        result.IsT0.Should().BeTrue();
        result.AsT0.Should().Contain(x => x.PropertyName == "Name");
    }

    [Fact]
    public async Task AddProductAsync_WhenProductExists_ReturnsValidationError()
    {
        using var context = CreateContext();
        context.Products.Add(new Product { Name = "Test" });
        await context.SaveChangesAsync();

        var generalRepo = new Mock<IGeneralRepository>();
        generalRepo.Setup(x => x.ValidateRequest(It.IsAny<IValidator<CreateProductVM>>(), It.IsAny<CreateProductVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var repo = CreateRepository(context, generalRepo.Object);

        var vm = new CreateProductVM("Test", string.Empty, default, default, default, new Mock<IFormFile>().Object);
        var result = await repo.AddProductAsync(vm);

        result.IsT0.Should().BeTrue();
        result.AsT0.Should().Contain(x => x.PropertyName == "Name");
    }

    [Fact]
    public async Task AddProductAsync_WhenImageUploadFails_ReturnsValidationError()
    {
        using var context = CreateContext();
        var generalRepo = new Mock<IGeneralRepository>();
        generalRepo.Setup(x => x.ValidateRequest(It.IsAny<IValidator<CreateProductVM>>(), It.IsAny<CreateProductVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        cloudinaryRepo.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync((string?)null);

        var repo = CreateRepository(context, generalRepo.Object, null, null, null, null, null, cloudinaryRepo.Object);

        var vm = new CreateProductVM ("New",string.Empty,default,default,default, new Mock<IFormFile>().Object);
        var result = await repo.AddProductAsync(vm);

        result.IsT0.Should().BeTrue();
        result.AsT0.Should().Contain(x => x.PropertyName == "Server Error");
    }

    [Fact]
    public async Task AddProductAsync_WhenProductAdded_ReturnsTrue()
    {
        using var context = CreateContext();
        var generalRepo = new Mock<IGeneralRepository>();
        generalRepo.Setup(x => x.ValidateRequest(It.IsAny<IValidator<CreateProductVM>>(), It.IsAny<CreateProductVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        cloudinaryRepo.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("imageUrl");

        var repo = CreateRepository(context, generalRepo.Object, null, null, null, null, null, cloudinaryRepo.Object);

        var vm = new CreateProductVM ("New", string.Empty, default, default, default, new Mock<IFormFile>().Object);
        var result = await repo.AddProductAsync(vm);

        result.IsT1.Should().BeTrue();
        context.Products.Count().Should().Be(1);
    }





    [Fact]
    public async Task UpdateProductAsync_WhenImageUpdateFails_ReturnsServerError()
    {
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Test", ImageName = "old.png", CategoryId = 1 });
        await context.SaveChangesAsync();

        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        cloudinaryRepo.Setup(x => x.UpdateImageAsync("old.png", It.IsAny<IFormFile>())).ReturnsAsync((string?)null);

        var repo = CreateRepository(context, null, null, null, null, null, null, cloudinaryRepo.Object);

        var vm = new EditProductVM { Id = 1, Name = "Test", ImageFile = new Mock<IFormFile>().Object };
        var result = await repo.UpdateProductAsync(vm);

        result.IsT0.Should().BeTrue();
        result.AsT0.Should().Contain(x => x.PropertyName == "Server Error");
    }

    [Fact]
    public async Task UpdateProductAsync_WhenProductUpdated_ReturnsTrue()
    {
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Test", ImageName = "old.png", CategoryId = 1 });
        await context.SaveChangesAsync();

        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        cloudinaryRepo.Setup(x => x.UpdateImageAsync("old.png", It.IsAny<IFormFile>())).ReturnsAsync("new.png");

        var repo = CreateRepository(context, null, null, null, null, null, null, cloudinaryRepo.Object);

        var vm = new EditProductVM { Id = 1, Name = "Test", ImageFile = new Mock<IFormFile>().Object };
        var result = await repo.UpdateProductAsync(vm);

        result.IsT1.Should().BeTrue();
        context.Products.First().ImageName.Should().Be("new.png");
    }




    [Fact]
    public async Task DeleteProductAsync_WhenProductNotFound_ReturnsFalse()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.DeleteProductAsync(99);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteProductAsync_WhenImageDeleteFails_ReturnsFalse()
    {
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Test", ImageName = "img.png" });
        await context.SaveChangesAsync();

        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        cloudinaryRepo.Setup(x => x.DeleteImageAsync("img.png")).ReturnsAsync(false);

        var repo = CreateRepository(context, null, null, null, null, null, null, cloudinaryRepo.Object);

        var result = await repo.DeleteProductAsync(1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductDeleted_ReturnsTrue()
    {
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Test", ImageName = "img.png" });
        await context.SaveChangesAsync();

        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        cloudinaryRepo.Setup(x => x.DeleteImageAsync("img.png")).ReturnsAsync(true);

        var repo = CreateRepository(context, null, null, null, null, null, null, cloudinaryRepo.Object);

        var result = await repo.DeleteProductAsync(1);

        result.Should().BeTrue();
        context.Products.Count().Should().Be(0);
    }





    [Fact]
    public async Task GetAllProductsAdminAsync_ReturnsPaginatedList()
    {
        using var context = CreateContext();
        context.Categories.Add(new Category { Id = 1, Name = "Cat1" });
        await context.SaveChangesAsync();
        context.Products.AddRange(
            [
                new Product { Id = 1, Name = "A", CategoryId = 1 },
                new Product { Id = 2, Name = "B", CategoryId = 1 },
                new Product { Id = 3, Name = "C", CategoryId = 1 }
            ]);
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var filter = new FilterRequest("", "name", "asc", 1);
        var result = await repo.GetAllProductsAdminAsync(filter);

        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
    }



    [Fact]
    public async Task GetProductEditByIdAsync_WhenExists_ReturnsProduct()
    {
        using var context = CreateContext();
        context.Categories.Add(new Category { Id = 1, Name = "Cat1" });
        await context.SaveChangesAsync();
        context.Products.Add(new Product { Id = 1, Name = "Test", CategoryId = 1 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetProductEditByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetProductEditByIdAsync_WhenNotExists_ReturnsNull()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.GetProductEditByIdAsync(99);

        result.Should().BeNull();
    }




    [Fact]
    public async Task GetProductDetailsByIdAsync_WhenExists_ReturnsProduct()
    {
        using var context = CreateContext();
        context.Categories.Add(new Category { Id = 1, Name = "Cat1" });
        await context.SaveChangesAsync();
        context.Products.Add(new Product { Id = 1, Name = "Test", CategoryId = 1 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetProductDetailsByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetProductDetailsByIdAsync_WhenNotExists_ReturnsNull()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.GetProductDetailsByIdAsync(99);

        result.Should().BeNull();
    }




    [Fact]
    public async Task GetDiscoverProductByIdAsync_WhenExists_ReturnsProduct()
    {
        using var context = CreateContext();
        context.Categories.Add(new Category { Id = 1, Name = "Cat1" });
        await context.SaveChangesAsync();
        context.Products.Add(new Product { Id = 1, Name = "Test", Description = "Desc", CategoryId = 1, Price = 10 });
        await context.SaveChangesAsync();
        var backgroundJobsRepo = new Mock<IBackgroundJobsRepository>().Object;
        var repo = CreateRepository(context,backgroundJobsRepository:backgroundJobsRepo);

        var result = await repo.GetDiscoverProductByIdAsync("user1", 1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetDiscoverProductByIdAsync_WhenNotExists_ReturnsNull()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.GetDiscoverProductByIdAsync("user1", 99);

        result.Should().BeNull();
    }



    [Fact]
    public async Task GetNewArrivalProductByIdAsync_WhenExists_ReturnsProduct()
    {
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Test", CategoryId = 1, Price = 10 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetNewArrivalProductByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetNewArrivalProductByIdAsync_WhenNotExists_ReturnsNull()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.GetNewArrivalProductByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetNewArrivalProductsAsync_ReturnsList()
    {
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Test", CategoryId = 1, Price = 10 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetNewArrivalProductsAsync("user1");

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }




    [Fact]
    public async Task GetBestSellingProductsAsync_ReturnsList()
    {
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Test", CategoryId = 1, Price = 10, SoldCount = 5 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetBestSellingProductsAsync("user1");

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }



    [Fact]
    public async Task GetDiscoverProductsAsync_ReturnsList()
    {
        using var context = CreateContext();
        context.Categories.Add(new Category { Id = 1, Name = "Cat1" });
        await context.SaveChangesAsync();
        context.Products.Add(new Product { Id = 1, Name = "Test", CategoryId = 1, Price = 10 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetDiscoverProductsAsync("user1");

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }



    [Fact]
    public async Task GetAllProductsSortedByAsync_ReturnsPaginatedList()
    {
        using var context = CreateContext();
        context.Categories.Add(new Category { Id = 1, Name = "Cat1" });
        await context.SaveChangesAsync();
        context.Products.Add(new Product { Id = 1, Name = "Test", CategoryId = 1, Price = 10, SoldCount = 5 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetAllProductsSortedByAsync("user1", "SoldCount", 1);

        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
    }



    [Fact]
    public async Task GetAllProductsInCategoryAsync_ReturnsPaginatedList()
    {
        using var context = CreateContext();
        context.Categories.Add(new Category { Id = 2, Name = "Cat1" });
        await context.SaveChangesAsync();
        context.Products.Add(new Product { Id = 1, Name = "Test", CategoryId = 2, Price = 10 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetAllProductsInCategoryAsync(1, 2);

        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
    }



    [Fact]
    public async Task SearchInProductsInHomeAsync_ReturnsPaginatedList()
    {
        using var context = CreateContext();
        context.Categories.Add(new Category { Id = 1, Name = "Cat1" });
        await context.SaveChangesAsync();
        context.Products.Add(new Product { Id = 1, Name = "TestProduct", Description = "Desc", CategoryId = 1, Price = 10 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.SearchInProductsInHomeAsync("Test");

        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
    }



    [Fact]
    public async Task GetRecommendationsProducts_ReturnsList()
    {
        using var context = CreateContext();
        context.Categories.Add(new Category { Id = 1, Name = "Cat1" });
        await context.SaveChangesAsync();
        context.Products.Add(new Product { Id = 1, Name = "Test", CategoryId = 1, Price = 10 });
        await context.SaveChangesAsync();

        var recRepo = new Mock<IRecommendationRepository>();
        recRepo.Setup(x => x.GetTopRecommendationsAsync("user1", default))
            .ReturnsAsync(new List<(int productId, float score)> { (1, 0.9f) });

        var repo = CreateRepository(context, null, null, null, null, null, recRepo.Object);

        var result = await repo.GetRecommendationsProducts("user1");

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }





}