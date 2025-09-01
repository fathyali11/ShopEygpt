using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Moq;
using System.Threading.Tasks;
using Web.DataAccess.Data;
using Web.DataAccess.Tests;
using Web.Entites.Consts;
using Web.Entites.IRepositories;
using Web.Entites.Models;
using Web.Entites.ViewModels.CartItemVMs;
using Xunit;

namespace Web.DataAccess.Repositories.Tests;

public class CartRepositoryTests
{
    [Fact()]
    public async Task GetCartItemCountAsync_WhenCartIsNotEmpty_ShouldReturnANumber()
    {
        // arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Carts.Add(new Cart { Id = 1, UserId = "user1", TotalPrice = 100.0m });
        await context.SaveChangesAsync();
        context.CartItems.AddRange(
            new CartItem { Id = 1, CartId = 1, ProductId = 1, ProductName = "Product1", Price = 50.0m, Count = 1 },
            new CartItem { Id = 2, CartId = 1, ProductId = 2, ProductName = "Product2", Price = 50.0m, Count = 1 }
        );
        await context.SaveChangesAsync();
        var fakeHybridCache = new FakeHybridCache();
        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, null!, fakeHybridCache,jobs);
        var userId = "user1";


        // act
        var result = await cartRepository.GetCartItemCountAsync(userId);

        // assert
        result.Should().Be(2);


    }
    [Fact()]
    public async Task GetCartItemCountAsync_WhenCartIsEmpty_ShouldReturnZero()
    {
        // arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Carts.Add(new Cart { Id = 1, UserId = "user1", TotalPrice = 100.0m });
        await context.SaveChangesAsync();
        var fakeHybridCache = new FakeHybridCache();

        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, null!, fakeHybridCache, jobs);
        var userId = "user1";


        // act
        var result = await cartRepository.GetCartItemCountAsync(userId);

        // assert
        result.Should().Be(0);


    }


    [Fact()]
    public async Task GetCartItemsAsync_WhenCartIsNull_ShouldReturnNewCartResponseVM()
    {
        //arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var fakeHybridCache = new FakeHybridCache();
        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, null!, fakeHybridCache, jobs);

        //act
        var result = await cartRepository.GetCartItemsAsync("user1");

        //assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalPrice.Should().Be(0.0m);
        result.Id.Should().Be(0);
    }
    [Fact()]
    public async Task GetCartItemsAsync_WhenCartIsNotNull_ShouldReturnCartConvertedIntoCartResponse()
    {
        //arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Carts.Add(new Cart { Id = 1, UserId = "user1", TotalPrice = 100.0m });
        await context.SaveChangesAsync();

        context.CartItems.AddRange(
            new CartItem { Id = 1, CartId = 1, ProductId = 1, ProductName = "Product1", Price = 50.0m, Count = 1 },
            new CartItem { Id = 2, CartId = 1, ProductId = 2, ProductName = "Product2", Price = 50.0m, Count = 1 }
        );
        await context.SaveChangesAsync();

        var fakeHybridCache = new FakeHybridCache();
        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, null!, fakeHybridCache, jobs);

        //act
        var result = await cartRepository.GetCartItemsAsync("user1");

        //assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalPrice.Should().Be(100.0m);
        result.Id.Should().Be(1);
    }


    [Fact()]
    public async Task IncreaseAsync_WhenCartItemExists_ShouldIncreaseCountByOne()
    {
        //arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        context.Carts.Add(new Cart { Id = 1, UserId = "user1", TotalPrice = 100.0m });
        await context.SaveChangesAsync();
        context.CartItems.AddRange(
            new CartItem { Id = 1, CartId = 1, ProductId = 1, ProductName = "Product1", Price = 50.0m, Count = 1 },
            new CartItem { Id = 2, CartId = 1, ProductId = 2, ProductName = "Product2", Price = 50.0m, Count = 1 }
        );
        await context.SaveChangesAsync();

        var fakeHybridCache = new FakeHybridCache();
        var logger = new Mock<ILogger<CartRepository>>().Object;
        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, logger, fakeHybridCache, jobs);
        var userId = "user1";
        var increaseItem = new Delete_Increase_DecreaseCartItemVM(1, 1);
        //act
        var result= await cartRepository.IncreaseAsync(userId, increaseItem);

        //assert
        var cartItem = await context.CartItems.FirstOrDefaultAsync(ci => ci.Id == 1);
        cartItem.Should().NotBeNull();
        cartItem!.Count.Should().Be(2);

        result.Item1.Should().Be(2);
        result.Item2.Should().Be(150.0m);
    }
    [Fact()]
    public async Task IncreaseAsync_WhenCartItemIsNotExists_ShouldReturnZero()
    {
        //arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);


        var fakeHybridCache = new FakeHybridCache();
        var logger = new Mock<ILogger<CartRepository>>().Object;
        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, logger, fakeHybridCache, jobs);
        var userId = "user1";
        var increaseItem = new Delete_Increase_DecreaseCartItemVM(1, 1);
        //act
        var result = await cartRepository.IncreaseAsync(userId, increaseItem);

        //assert
        result.Item1.Should().Be(0);
        result.Item2.Should().Be(0.0m);
    }


    // add tests for DecreaseAsync and DeleteCartItemAsync methods as needed
    [Fact()]
    public async Task DecreaseAsync_WhenCartItemExistsAndCountGreaterThanOne_ShouldDecreaseCountByOne()
    {
        //arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Carts.Add(new Cart { Id = 1, UserId = "user1", TotalPrice = 150.0m });
        await context.SaveChangesAsync();
        context.CartItems.AddRange(
            new CartItem { Id = 1, CartId = 1, ProductId = 1, ProductName = "Product1", Price = 50.0m, Count = 2 },
            new CartItem { Id = 2, CartId = 1, ProductId = 2, ProductName = "Product2", Price = 50.0m, Count = 1 }
        );
        await context.SaveChangesAsync();
        var fakeHybridCache = new FakeHybridCache();
        var logger = new Mock<ILogger<CartRepository>>().Object;
        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, logger, fakeHybridCache, jobs);
        var userId = "user1";
        var decreaseItem = new Delete_Increase_DecreaseCartItemVM(1, 1);
        //act
        var result = await cartRepository.DecreaseAsync(userId, decreaseItem);
        //assert
        var cartItem = await context.CartItems.FirstOrDefaultAsync(ci => ci.Id == 1);
        cartItem.Should().NotBeNull();
        cartItem!.Count.Should().Be(1);
        result.Item1.Should().Be(1);
        result.Item2.Should().Be(100.0m);
    }
    [Fact()]
    public async Task DecreaseAsync_WhenCartItemExistsAndCountIsOne_ShouldRemoveCartItem()
    {
        // arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Carts.Add(new Cart { Id = 1, UserId = "user1", TotalPrice = 100.0m });
        await context.SaveChangesAsync();
        context.CartItems.AddRange(
            new CartItem { Id = 1, CartId = 1, ProductId = 1, ProductName = "Product1", Price = 50.0m, Count = 1 },
            new CartItem { Id = 2, CartId = 1, ProductId = 2, ProductName = "Product2", Price = 50.0m, Count = 1 }
        );
        await context.SaveChangesAsync();
        
        var fakeHybridCache = new FakeHybridCache();
        var logger = new Mock<ILogger<CartRepository>>().Object;
        var jobs = new Mock<IBackgroundJobsRepository>();
        var cartRepository = new CartRepository(context, logger, fakeHybridCache,jobs.Object);
        var userId = "user1";
        var decreaseItem = new Delete_Increase_DecreaseCartItemVM(1, 1);
        //act
        var result = await cartRepository.DecreaseAsync(userId, decreaseItem);
        //assert
        var cartItem = await context.CartItems.FirstOrDefaultAsync(ci => ci.Id == 1);
        cartItem.Should().BeNull();
    }
    [Fact]
    public async Task DecreaseAsync_WhenCartItemDoesNotExist_ShouldReturnZero()
    {
        //arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var fakeHybridCache = new FakeHybridCache();
        var logger = new Mock<ILogger<CartRepository>>().Object;
        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, logger, fakeHybridCache, jobs);
        var userId = "user1";
        var decreaseItem = new Delete_Increase_DecreaseCartItemVM(1, 1);
        //act
        var result = await cartRepository.DecreaseAsync(userId, decreaseItem);
        //assert
        result.Item1.Should().Be(0);
        result.Item2.Should().Be(0.0m);
    }
    [Fact]
    public async Task DeleteCartItemAsync_WhenCartItemExists_ShouldRemoveItemAndUpdateTotalPrice()
    {
        //arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Carts.Add(new Cart { Id = 1, UserId = "user1", TotalPrice = 150.0m });
        await context.SaveChangesAsync();
        context.CartItems.AddRange(
            new CartItem { Id = 1, CartId = 1, ProductId = 1, ProductName = "Product1", Price = 50.0m, Count = 2 },
            new CartItem { Id = 2, CartId = 1, ProductId = 2, ProductName = "Product2", Price = 50.0m, Count = 1 }
        );
        await context.SaveChangesAsync();
        var fakeHybridCache = new FakeHybridCache();
        var logger = new Mock<ILogger<CartRepository>>().Object;
        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, logger, fakeHybridCache, jobs);
        var userId = "user1";
        var deleteItem = new Delete_Increase_DecreaseCartItemVM(1, 1);
        //act
        await cartRepository.DeleteCartItemAsync(userId, deleteItem);
        //assert
        var cartItem = await context.CartItems.FirstOrDefaultAsync(ci => ci.Id == 1);
        cartItem.Should().BeNull();
        var cart = await context.Carts.FirstOrDefaultAsync(c => c.Id == 1);
        cart.Should().NotBeNull();
        cart!.TotalPrice.Should().Be(50.0m);
    }
    [Fact]
    public async Task DeleteCartItemAsync_WhenCartItemDoesNotExist_ShouldDoNothing()
    {
        //arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        context.Carts.Add(new Cart { Id = 1, UserId = "user1", TotalPrice = 150.0m });
        await context.SaveChangesAsync();
        context.CartItems.AddRange(
            new CartItem { Id = 1, CartId = 1, ProductId = 1, ProductName = "Product1", Price = 50.0m, Count = 2 },
            new CartItem { Id = 2, CartId = 1, ProductId = 2, ProductName = "Product2", Price = 50.0m, Count = 1 }
        );
        await context.SaveChangesAsync();
        var fakeHybridCache = new FakeHybridCache();
        var logger = new Mock<ILogger<CartRepository>>().Object;
        var jobs = new Mock<IBackgroundJobsRepository>().Object;
        var cartRepository = new CartRepository(context, logger, fakeHybridCache, jobs);
        var userId = "user1";
        var deleteItem = new Delete_Increase_DecreaseCartItemVM(3, 1); // Non-existent cart item ID
        //act
        await cartRepository.DeleteCartItemAsync(userId, deleteItem);
        //assert
        var cartItems = await context.CartItems.ToListAsync();
        cartItems.Should().HaveCount(2); // No items should be removed
        var cart = await context.Carts.FirstOrDefaultAsync(c => c.Id == 1);
        cart.Should().NotBeNull();
        cart!.TotalPrice.Should().Be(150.0m); // Total price should remain unchanged
    }

}