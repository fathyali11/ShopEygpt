namespace Web.DataAccess.Repositories.Tests;
public class CategoryRepositoryTests
{
    [Fact]
    public async Task AddCategoryAsync_WhenValid_ShouldAddCategory()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        generalRepo.Setup(x => x.ValidateRequest<IValidator<CreateCategoryVM>, CreateCategoryVM>(
            It.IsAny<IValidator<CreateCategoryVM>>(), It.IsAny<CreateCategoryVM>()))
            .ReturnsAsync((List<ValidationError>?)null);
        cloudinaryRepo.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("imageUrl");
        productRepo.Setup(x => x.RemoveKeys(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var categoryVM = new CreateCategoryVM ( "Test",  null !);
        var result = await repo.AddCategoryAsync(categoryVM);

        result.IsT1.Should().BeTrue();
        context.Categories.Count().Should().Be(1);
        context.Categories.First().ImageName.Should().Be("imageUrl");
    }

    [Fact]
    public async Task AddCategoryAsync_WhenDuplicateName_ShouldReturnValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        context.Categories.Add(new Category { Name = "Test" });
        await context.SaveChangesAsync();

        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        generalRepo.Setup(x => x.ValidateRequest<IValidator<CreateCategoryVM>, CreateCategoryVM>(
            It.IsAny<IValidator<CreateCategoryVM>>(), It.IsAny<CreateCategoryVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var categoryVM = new CreateCategoryVM ("Test", null! );
        var result = await repo.AddCategoryAsync(categoryVM);

        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("Duplicate Category");
        error.ErrorMessage.Should().Be("Category with this name already exists");

    }
    [Fact]
    public async Task AddCategoryAsync_WhenRequestNotValid_ShouldReturnValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);


        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        generalRepo.Setup(x => x.ValidateRequest<IValidator<CreateCategoryVM>, CreateCategoryVM>(
            It.IsAny<IValidator<CreateCategoryVM>>(), It.IsAny<CreateCategoryVM>()))
            .ReturnsAsync(new List<ValidationError>());

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var categoryVM = new CreateCategoryVM("Test", null!);
        var result = await repo.AddCategoryAsync(categoryVM);

        result.IsT0.Should().BeTrue();
        result.AsT0.Should().NotBeNull();

    }

    [Fact]
    public async Task AddCategoryAsync_WhenSaveImageFail_ShouldReturnValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);


        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        generalRepo.Setup(x => x.ValidateRequest<IValidator<CreateCategoryVM>, CreateCategoryVM>(
            It.IsAny<IValidator<CreateCategoryVM>>(), It.IsAny<CreateCategoryVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        cloudinaryRepo.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync((string?)null);

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var categoryVM = new CreateCategoryVM("Test", null!);
        var result = await repo.AddCategoryAsync(categoryVM);

        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("Server Error");
        error.ErrorMessage.Should().Be("Internal server error in saving image");

    }





    [Fact]
    public async Task GetCategoryAsync_WhenExists_ShouldReturnCategoryResponse()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var category = new Category { Name = "Test" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var result = await repo.GetCategoryAsync(category.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }
    [Fact]
    public async Task GetCategoryAsync_WhenNotExists_ShouldReturnNull()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();
        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );
        var result = await repo.GetCategoryAsync(1);
        result.Should().BeNull();
    }






    [Fact]
    public async Task UpdateCategoryAsync_WhenValid_ShouldUpdateCategory()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var category = new Category { Name = "Test", ImageName = "oldImage" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        generalRepo.Setup(x => x.ValidateRequest(It.IsAny<IValidator<EditCategoryVM>>(), It.IsAny<EditCategoryVM>()))
            .ReturnsAsync((List<ValidationError>?)null);
        cloudinaryRepo.Setup(x => x.UpdateImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>())).ReturnsAsync("newImageUrl");
        productRepo.Setup(x => x.RemoveKeys(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var editVM = new EditCategoryVM(category.Id, "Updated",string.Empty, null);
        var result = await repo.UpdateCategoryAsync(editVM);

        result.IsT1.Should().BeTrue();
        context.Categories.First().Name.Should().Be("Updated");
    }
    [Fact]
    public async Task UpdateCategoryAsync_WhenNotValid_ShouldReturnValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();
        generalRepo.Setup(x => x.ValidateRequest(It.IsAny<IValidator<EditCategoryVM>>(), It.IsAny<EditCategoryVM>()))
            .ReturnsAsync(new List<ValidationError>());
        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );
        var editVM = new EditCategoryVM(1, "Updated", string.Empty, null);
        var result = await repo.UpdateCategoryAsync(editVM);
        result.IsT0.Should().BeTrue();
        result.AsT0.Should().NotBeNull();
    }
    [Fact]
    public async Task UpdateCategoryAsync_WhenCategoryNotFound_ShouldReturnValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();
        generalRepo.Setup(x => x.ValidateRequest(It.IsAny<IValidator<EditCategoryVM>>(), It.IsAny<EditCategoryVM>()))
            .ReturnsAsync((List<ValidationError>?)null);
        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );
        var editVM = new EditCategoryVM(1, "Updated", string.Empty, null);
        var result = await repo.UpdateCategoryAsync(editVM);
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("Not Found");
        error.ErrorMessage.Should().Be("Category not found");
    }
    [Fact]
    public async Task UpdateCategoryAsync_WhenImageUpdateFails_ShouldReturnValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var category = new Category { Name = "Test", ImageName = "oldImage" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();
        generalRepo.Setup(x => x.ValidateRequest(It.IsAny<IValidator<EditCategoryVM>>(), It.IsAny<EditCategoryVM>()))
            .ReturnsAsync((List<ValidationError>?)null);
        cloudinaryRepo.Setup(x => x.UpdateImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>())).ReturnsAsync((string?)null);
        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );
        var editVM = new EditCategoryVM(category.Id, "Updated", string.Empty,
            new FormFile(Stream.Null, 0, 0, "Image", "default.png"));
        var result = await repo.UpdateCategoryAsync(editVM);
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("Server Error");
        error.ErrorMessage.Should().Be("Internal server error in saving image");
    }







    [Fact]
    public async Task DeleteCategoryAsync_WhenItFoundAndHasNoProducts_ShouldDeleteCategory()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var category = new Category { Name = "Test", ImageName = "img" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        cloudinaryRepo.Setup(x => x.DeleteImageAsync(It.IsAny<string>())).ReturnsAsync(true);
        productRepo.Setup(x => x.RemoveKeys(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var result = await repo.DeleteCategoryAsync(category.Id);

        result.IsT1.Should().BeTrue();
        context.Categories.Count().Should().Be(0);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenCategoryHasProducts_ShouldReturnValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var category = new Category { Name = "Test", ImageName = "img" };
        context.Categories.Add(category);
        context.Products.Add(new Product { CategoryId = category.Id });
        await context.SaveChangesAsync();

        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var result = await repo.DeleteCategoryAsync(category.Id);

        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("Cannot Delete");
        error.ErrorMessage.Should().Be("Category cannot be deleted as it has associated products");
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenCategoryNotFound_ShouldReturnValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();
        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );
        var result = await repo.DeleteCategoryAsync(1);
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("Not Found");
        error.ErrorMessage.Should().Be("Category not found");
    }
    [Fact]
    public async Task DeleteCategoryAsync_WhenImageDeletionFails_ShouldReturnValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var category = new Category { Name = "Test", ImageName = "img" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();
        cloudinaryRepo.Setup(x => x.DeleteImageAsync(It.IsAny<string>())).ReturnsAsync(false);
        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );
        var result = await repo.DeleteCategoryAsync(category.Id);
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("Server Error");
        error.ErrorMessage.Should().Be("Internal server error in deleting image");
    }






    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnPaginatedList()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        context.Categories.Add(new Category { Name = "Test1" });
        context.Categories.Add(new Category { Name = "Test2" });
        await context.SaveChangesAsync();

        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var filter = new FilterRequest(null, "name", "asc", 1);
        var result = await repo.GetAllCategoriesAsync(filter);

        result.Items.Count.Should().BeGreaterThan(0);
    }





    // create tests for GetAllCategoriesInHomeAsync
    [Fact]
    public async Task GetAllCategoriesInHomeAsync_WhenChooseAllCategories_ShouldReturnPaginateCategories()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var categories = new List<Category>
        {
            new Category { Name = "Test1" },
            new Category { Name = "Test2" },
            new Category { Name = "Test3" },
            new Category { Name = "Test4" },
            new Category { Name = "Test5" }
        };
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();
        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );
        var result = await repo.GetAllCategoriesInHomeAsync(true,1);
        result.IsT0.Should().BeTrue();
        result.AsT0.Items.Count.Should().BeGreaterThan(0);
    }
    [Fact]
    public async Task GetAllCategoriesInHomeAsync_WhenNotChooseAllCategories_ShouldReturnCategoryInHomeVM()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var categories = new List<Category>
        {
            new Category { Name = "Test1" },
            new Category { Name = "Test2" },
            new Category { Name = "Test3" },
            new Category { Name = "Test4" },
            new Category { Name = "Test5" }
        };
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();
        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );
        var result = await repo.GetAllCategoriesInHomeAsync(false, 1);
        result.IsT1.Should().BeTrue();
        result.AsT1.Count.Should().BeGreaterThan(0);
    }




    [Fact]
    public async Task GetAllCategoriesSelectListAsync_ShouldReturnSelectList()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        context.Categories.Add(new Category { Name = "Test1" });
        await context.SaveChangesAsync();

        var generalRepo = new Mock<IGeneralRepository>();
        var createValidator = new Mock<IValidator<CreateCategoryVM>>();
        var editValidator = new Mock<IValidator<EditCategoryVM>>();
        var cloudinaryRepo = new Mock<ICloudinaryRepository>();
        var productRepo = new Mock<IProductRepository>();
        var fakeCache = new FakeHybridCache();

        var repo = new CategoryRepository(
            context,
            generalRepo.Object,
            createValidator.Object,
            editValidator.Object,
            fakeCache,
            cloudinaryRepo.Object,
            productRepo.Object
        );

        var result = await repo.GetAllCategoriesSelectListAsync();

        result.Should().NotBeNull();
        result.Should().Contain(x => x.Text == "Test1");
    }
}