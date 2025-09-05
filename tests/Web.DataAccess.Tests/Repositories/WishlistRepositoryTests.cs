namespace Web.DataAccess.Repositories.Tests;
public class WishlistRepositoryTests
{
    [Fact]
    public async Task ToggelWishlistItemAsync_WhenProductFoundInWishlist_ShouldRemoveItAndReturnFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var fakeCache = new FakeHybridCache();
        var backgroundJobRepo = new Mock<IBackgroundJobsRepository>();

        var wishlist = new Wishlist
        {
            Id = 1,
            UserId = "user1"
        };
        context.Wishlist.Add(wishlist);
        await context.SaveChangesAsync();

        var wishlistItem = new WishlistItem
        {
            WishlistId = 1,
            ProductId = 1,
            Price = 29.99m,
            ProductName = "Test Product",
            ImageName = "test.jpg"
        };
        context.WishlistItems.Add(wishlistItem);
        await context.SaveChangesAsync();

        var repository = new WishlistRepository(context, fakeCache, backgroundJobRepo.Object);
        var addWishlistItem = new AddWishlistItem(
            1,
            "Test Product",
            "test.jpg",
            29.99m,
            true
        );

        // Act
        var result = await repository.ToggelWishlistItemAsync("user1", addWishlistItem);

        // Assert
        result.Should().BeFalse();
        var itemExists = await context.WishlistItems.AnyAsync(x =>
            x.WishlistId == wishlist.Id &&
            x.ProductId == addWishlistItem.ProductId);
        itemExists.Should().BeFalse();
        backgroundJobRepo.Verify(x => x.Enqueue<IProductRatingRepository>(
            It.IsAny<Expression<Action<IProductRatingRepository>>>()),
            Times.Once);
    }
    [Fact]
    public async Task ToggelWishlistItemAsync_WhenProductNotInWishlist_ShouldAddItAndReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var fakeCache = new FakeHybridCache();
        var backgroundJobRepo = new Mock<IBackgroundJobsRepository>();

        var wishlist = new Wishlist
        {
            Id = 1,
            UserId = "user1"
        };
        context.Wishlist.Add(wishlist);
        await context.SaveChangesAsync();

        var repository = new WishlistRepository(context, fakeCache, backgroundJobRepo.Object);
        var addWishlistItem = new AddWishlistItem(
            1,
            "Test Product",
            "test.jpg",
            29.99m,
            true
        );

        // Act
        var result = await repository.ToggelWishlistItemAsync("user1", addWishlistItem);

        // Assert
        result.Should().BeTrue();
        var addedItem = await context.WishlistItems.FirstOrDefaultAsync(x =>
            x.WishlistId == wishlist.Id &&
            x.ProductId == addWishlistItem.ProductId);
        addedItem.Should().NotBeNull();
        addedItem!.ProductName.Should().Be("Test Product");
        addedItem.Price.Should().Be(29.99m);
        backgroundJobRepo.Verify(x => x.Enqueue<IProductRatingRepository>(
            It.IsAny<Expression<Action<IProductRatingRepository>>>()),
            Times.Once);
    }



    
    [Fact]
    public async Task GetWishlistItems_WhenWishlistFound_ShouldReturnWishlistItems()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var fakeCache = new FakeHybridCache();
        var backgroundJobRepo = new Mock<IBackgroundJobsRepository>();
        var wishlist = new Wishlist
        {
            Id = 1,
            UserId = "user1"
        };
        context.Wishlist.Add(wishlist);
        await context.SaveChangesAsync();
        var wishlistItem1 = new WishlistItem
        {
            WishlistId = 1,
            ProductId = 1,
            Price = 29.99m,
            ProductName = "Test Product 1",
            ImageName = "test1.jpg"
        };
        var wishlistItem2 = new WishlistItem
        {
            WishlistId = 1,
            ProductId = 2,
            Price = 39.99m,
            ProductName = "Test Product 2",
            ImageName = "test2.jpg"
        };
        context.WishlistItems.AddRange(wishlistItem1, wishlistItem2);
        await context.SaveChangesAsync();
        var repository = new WishlistRepository(context, fakeCache, backgroundJobRepo.Object);
        // Act
        var result = await repository.GetWishlistItems("user1");
        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().ContainSingle(x => x.ProductId == 1 && x.ProductName == "Test Product 1");
        result.Items.Should().ContainSingle(x => x.ProductId == 2 && x.ProductName == "Test Product 2");
    }
    [Fact]
    public async Task GetWishlistItems_WhenNoWishlistFound_ShouldReturnEmptyList()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var fakeCache = new FakeHybridCache();
        var backgroundJobRepo = new Mock<IBackgroundJobsRepository>();
        var repository = new WishlistRepository(context, fakeCache, backgroundJobRepo.Object);
        // Act
        var result = await repository.GetWishlistItems("nonexistentuser");
        // Assert
        result.Items.Should().BeEmpty();
    }




    [Fact]
    public async Task GetWishlistItemCountAsync_WhenWishlistFound_ShouldReturnCorrectCount()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var fakeCache = new FakeHybridCache();
        var backgroundJobRepo = new Mock<IBackgroundJobsRepository>();
        var wishlist = new Wishlist
        {
            Id = 1,
            UserId = "user1"
        };
        context.Wishlist.Add(wishlist);
        await context.SaveChangesAsync();
        var wishlistItem1 = new WishlistItem
        {
            WishlistId = 1,
            ProductId = 1,
            Price = 29.99m,
            ProductName = "Test Product 1",
            ImageName = "test1.jpg"
        };
        var wishlistItem2 = new WishlistItem
        {
            WishlistId = 1,
            ProductId = 2,
            Price = 39.99m,
            ProductName = "Test Product 2",
            ImageName = "test2.jpg"
        };
        context.WishlistItems.AddRange(wishlistItem1, wishlistItem2);
        await context.SaveChangesAsync();
        var repository = new WishlistRepository(context, fakeCache, backgroundJobRepo.Object);
        // Act
        var result = await repository.GetWishlistItemCountAsync("user1");
        // Assert
        result.Should().Be(2);
    }
    [Fact]
    public async Task GetWishlistItemCountAsync_WhenNoWishlistFound_ShouldReturnZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var fakeCache = new FakeHybridCache();
        var backgroundJobRepo = new Mock<IBackgroundJobsRepository>();
        var repository = new WishlistRepository(context, fakeCache, backgroundJobRepo.Object);
        // Act
        var result = await repository.GetWishlistItemCountAsync("nonexistentuser");
        // Assert
        result.Should().Be(0);
    }



    // add tests for DeleteWishlistItemAsync 
    [Fact]
    public async Task DeleteWishlistItemAsync_WhenWishlistItemDeletedSuccessfully_ShouldReturnWishlistCount()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Wishlist.Add(new Wishlist { Id = 1 ,UserId="user1"});
        context.SaveChanges();
        context.WishlistItems.Add(new WishlistItem
        {
            WishlistId = 1,
            ProductId = 1,
            Price = 29.99m,
            ProductName = "Test Product 1",
            ImageName = "test1.jpg"
        });
        context.SaveChanges();
        var fakeCache = new FakeHybridCache();
        var backgroundJobRepo = new Mock<IBackgroundJobsRepository>();
        var repository = new WishlistRepository(context, fakeCache, backgroundJobRepo.Object);
        var model = new DeleteWishlistItem(1, 1);

        // act
        var result = await repository.DeleteWishlistItemAsync("user1", model);

        // assert
        result.Should().Be(0);
        backgroundJobRepo.Verify(x => x.Enqueue<IProductRatingRepository>(
            It.IsAny<Expression<Action<IProductRatingRepository>>>()),
            Times.Once);
    }
    [Fact]
    public async Task DeleteWishlistItemAsync_WhenWishlistItemNotFound_ShouldReturnMinusOne()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Wishlist.Add(new Wishlist { Id = 1, UserId = "user1" });
        context.SaveChanges();
        var fakeCache = new FakeHybridCache();
        var backgroundJobRepo = new Mock<IBackgroundJobsRepository>();
        var repository = new WishlistRepository(context, fakeCache, backgroundJobRepo.Object);
        var model = new DeleteWishlistItem(1, 1);
        // act
        var result = await repository.DeleteWishlistItemAsync("user1", model);
        // assert
        result.Should().Be(-1);
    }
}