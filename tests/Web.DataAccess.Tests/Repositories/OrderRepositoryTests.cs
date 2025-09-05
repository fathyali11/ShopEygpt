using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Web.DataAccess.Data;
using Web.DataAccess.Tests;
using Web.Entites.Consts;
using Web.Entites.IRepositories;
using Web.Entites.Models;
using Xunit;

namespace Web.DataAccess.Repositories.Tests;

public class OrderRepositoryTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private OrderRepository CreateRepository(
        ApplicationDbContext context,
        IPaymentRepository? paymentRepo = null,
        FakeHybridCache? cache = null,
        ILogger<OrderRepository>? logger = null,
        IBackgroundJobsRepository ?backgroundJobsRepository=null)
    {
        return new OrderRepository(
            context,
            paymentRepo ?? new Mock<IPaymentRepository>().Object,
            cache ?? new FakeHybridCache(),
            logger ?? new Mock<ILogger<OrderRepository>>().Object,
            backgroundJobsRepository??new Mock<IBackgroundJobsRepository>().Object
        );
    }

    [Fact]
    public async Task GetAllOrdersAsync_WhenOrdersExist_ShouldReturnsOrders()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = "Paid", TotalPrice = 100 });
        context.Orders.Add(new Order { Id = 2, UserId = "user2", Status = "Shipped", TotalPrice = 200 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var filter = new FilterRequest ("", "status", "asc", 1);
        var result = await repo.GetAllOrdersAsync(filter);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllOrdersAsync_WhenNoOrdersExist_ReturnsEmpty()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);

        var filter = new FilterRequest ("", "status", "asc", 1);
        var result = await repo.GetAllOrdersAsync(filter);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }





    [Fact]
    public async Task CreateOrderAsync_CreatesOrder_WhenCartExists()
    {
        using var context = CreateContext();
        context.Carts.Add(new Cart
        {
            Id = 1,
            UserId = "user1",
            TotalPrice = 100,
            CartItems = new List<CartItem> {
            new CartItem { CartId = 1, ProductId = 1, Price = 50, Count = 1 },
            new CartItem { CartId = 1, ProductId = 2, Price = 50, Count = 1 }
        }
        });
        await context.SaveChangesAsync();

        var backgroundJobsRepo = new Mock<IBackgroundJobsRepository>().Object;

        var repo = CreateRepository(context,backgroundJobsRepository:backgroundJobsRepo);

        var result = await repo.CreateOrderAsync("user1", "pi_123", "sess_123");

        result.Should().NotBeNull();
        context.Orders.Count().Should().Be(1);
        context.Carts.Count().Should().Be(0);
    }

    [Fact]
    public async Task CreateOrderAsync_ReturnsNull_WhenCartDoesNotExist()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.CreateOrderAsync("user1", "pi_123", "sess_123");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrderDetailsAsync_ReturnsDetails_WhenOrderExists()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = "Paid", TotalPrice = 100 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetOrderDetailsAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetOrderDetailsAsync_ReturnsNull_WhenOrderDoesNotExist()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.GetOrderDetailsAsync(99);

        result.Should().BeNull();
    }






    [Fact]
    public async Task CancelOrderAsync_ReturnsFalse_WhenOrderDoesNotExist()
    {
        using var context = CreateContext();
        var paymentRepo = new Mock<IPaymentRepository>();
        var repo = CreateRepository(context, paymentRepo.Object);

        var result = await repo.CancelOrderAsync("user1", 99);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelOrderAsync_ReturnsFalse_WhenOrderAlreadyCancelled()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = OrderStatus.Cancelled, TotalPrice = 100 });
        await context.SaveChangesAsync();

        var paymentRepo = new Mock<IPaymentRepository>();
        var repo = CreateRepository(context, paymentRepo.Object);

        var result = await repo.CancelOrderAsync("user1", 1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelOrderAsync_ReturnsFalse_WhenRefundFails()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = "Paid", PaymentIntentId = "pi_123", TotalPrice = 100 });
        await context.SaveChangesAsync();

        var paymentRepo = new Mock<IPaymentRepository>();
        paymentRepo.Setup(x => x.RefundPaymentAsync("pi_123", default)).ReturnsAsync(false);

        var repo = CreateRepository(context, paymentRepo.Object);

        var result = await repo.CancelOrderAsync("user1", 1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelOrderAsync_UpdatesStatusAndReturnsTrue_WhenRefundSucceeds()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = "Paid", PaymentIntentId = "pi_123", TotalPrice = 100 });
        await context.SaveChangesAsync();

        var paymentRepo = new Mock<IPaymentRepository>();
        paymentRepo.Setup(x => x.RefundPaymentAsync("pi_123", default)).ReturnsAsync(true);

        var repo = CreateRepository(context, paymentRepo.Object);

        var result = await repo.CancelOrderAsync("user1", 1);

        result.Should().BeTrue();
        context.Orders.Find(1)!.Status.Should().Be(OrderStatus.Cancelled);
    }






    [Fact]
    public async Task DeleteOrderAsync_ReturnsFalse_WhenOrderDoesNotExist()
    {
        using var context = CreateContext();
        var paymentRepo = new Mock<IPaymentRepository>();
        var repo = CreateRepository(context, paymentRepo.Object);

        var result = await repo.DeleteOrderAsync("user1", 99);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteOrderAsync_ReturnsFalse_WhenOrderAlreadyDeleted()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = OrderStatus.Deleted, TotalPrice = 100 });
        await context.SaveChangesAsync();

        var paymentRepo = new Mock<IPaymentRepository>();
        var repo = CreateRepository(context, paymentRepo.Object);

        var result = await repo.DeleteOrderAsync("user1", 1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteOrderAsync_ReturnsFalse_WhenRefundFails()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = "Paid", PaymentIntentId = "pi_123", TotalPrice = 100 });
        await context.SaveChangesAsync();

        var paymentRepo = new Mock<IPaymentRepository>();
        paymentRepo.Setup(x => x.RefundPaymentAsync("pi_123", default)).ReturnsAsync(false);

        var repo = CreateRepository(context, paymentRepo.Object);

        var result = await repo.DeleteOrderAsync("user1", 1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteOrderAsync_RemovesOrderAndReturnsTrue_WhenRefundSucceeds()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = "Paid", PaymentIntentId = "pi_123", TotalPrice = 100 });
        await context.SaveChangesAsync();

        var paymentRepo = new Mock<IPaymentRepository>();
        paymentRepo.Setup(x => x.RefundPaymentAsync("pi_123", default)).ReturnsAsync(true);

        var repo = CreateRepository(context, paymentRepo.Object);

        var result = await repo.DeleteOrderAsync("user1", 1);

        result.Should().BeTrue();
        context.Orders.Find(1).Should().BeNull();
    }

    [Fact]
    public async Task DeleteOrderAsync_RemovesOrderAndReturnsTrue_WhenOrderNotPaid()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = "Shipped", TotalPrice = 100 });
        await context.SaveChangesAsync();

        var paymentRepo = new Mock<IPaymentRepository>();
        var repo = CreateRepository(context, paymentRepo.Object);

        var result = await repo.DeleteOrderAsync("user1", 1);

        result.Should().BeTrue();
        context.Orders.Find(1).Should().BeNull();
    }





    [Fact]
    public async Task GetCurrentOrdersForUserAsync_ReturnsActiveOrders()
    {
        using var context = CreateContext();
        context.Orders.Add(new Order { Id = 1, UserId = "user1", Status = OrderStatus.Paid, TotalPrice = 100 });
        context.Orders.Add(new Order { Id = 2, UserId = "user1", Status = OrderStatus.Cancelled, TotalPrice = 200 });
        context.Orders.Add(new Order { Id = 3, UserId = "user1", Status = OrderStatus.Deleted, TotalPrice = 300 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        var result = await repo.GetCurrentOrdersForUserAsync("user1");

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
    }
}