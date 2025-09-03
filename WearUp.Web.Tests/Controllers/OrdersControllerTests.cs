using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Web.Entites.Consts;
using Web.Entites.IRepositories;
using Web.Entites.ViewModels.OrderVMs;
using Xunit;
namespace WearUp.Web.Controllers.Tests;

public class OrdersControllerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _controller = new OrdersController(_orderRepositoryMock.Object);

        // Fake User
        var claim = new Claim(ClaimTypes.NameIdentifier, "user");
        var claimIdentity = new ClaimsIdentity([claim],"mock");
        var user = new ClaimsPrincipal(claimIdentity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

    }

    [Fact]
    public async Task Index_WhenCalled_ShouldReturnViewWithOrders()
    {
        // Arrange
        var orders = new List<OrderResponseVM>
        {
            new(){Id=1,UserId="user1",UserName="username1"},
            new(){Id=2,UserId="user2",UserName="username2"},
            new(){Id=3,UserId="user3",UserName="username3"},
            new(){Id=4,UserId="user4",UserName="username4"}
        };
        var paginatedOrders = PaginatedList<OrderResponseVM>.Create(orders, 1, 10);
        _orderRepositoryMock
            .Setup(r => r.GetAllOrdersAsync(It.IsAny<FilterRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedOrders);
        var request = new FilterRequest(null, null, null);
        // Act
        var result = await _controller.Index(request, CancellationToken.None);

        // Assert
        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.Model.Should().BeEquivalentTo(paginatedOrders);
    }

    [Fact]
    public async Task Details_WhenCalled_ShouldReturnOrderDetails()
    {
        // Arrange
        var orderDetails = new OrderDetailsReponseVM { Id = 1, OrderItems = [] };
        _orderRepositoryMock
            .Setup(r => r.GetOrderDetailsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderDetails);

        // Act
        var result = await _controller.Details(1, CancellationToken.None);

        // Assert
        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.Model.Should().Be(orderDetails);
    }

    [Fact]
    public async Task Cancel_WhenSuccess_ShouldReturnJsonSuccess()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(r => r.CancelOrderAsync(It.IsAny<string>(), 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Cancel(1, CancellationToken.None);

        // Assert
        var json = result.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeEquivalentTo(new { Success = true, message = "Order Cancelled Successfull" });
    }

    [Fact]
    public async Task Cancel_WhenFail_ShouldReturnJsonFailure()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(r => r.CancelOrderAsync("user-123", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Cancel(1, CancellationToken.None);

        // Assert
        var json = result.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeEquivalentTo(new { Success = false, message = "Order Not Cancelled" });
    }

    [Fact]
    public async Task Delete_WhenSuccess_ShouldReturnJsonSuccess()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(r => r.DeleteOrderAsync(It.IsAny<string>(), 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        var json = result.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeEquivalentTo(new { Success = true, message = "Order Deleted Successfull" });
    }

    [Fact]
    public async Task Delete_WhenFail_ShouldReturnJsonFailure()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(r => r.DeleteOrderAsync("user-123", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        var json = result.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeEquivalentTo(new { Success = false, message = "Order Not Deleted" });
    }

    [Fact]
    public async Task CurrentOrders_WhenCalled_ShouldReturnOrdersForUser()
    {
        // Arrange
        var orders = new List<OrderProfileVM>
        {
            new (){Id = 1},
            new (){Id = 2},
            new (){Id = 3},
            new (){Id = 4},
        };
        _orderRepositoryMock
            .Setup(r => r.GetCurrentOrdersForUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _controller.CurrentOrders(CancellationToken.None);

        // Assert
        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.Model.Should().BeEquivalentTo(orders);
    }

    [Fact]
    public async Task UpdateStatus_WhenCalled_ShouldReturnJsonSuccess()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(r => r.UpdateOrderStatusAsync(1, "Shipped", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateStatus(1, "Shipped", CancellationToken.None);

        // Assert
        var json = result.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeEquivalentTo(new { Success = true, message = "Order Status Updated Successfully" });
    }

    [Fact]
    public void Failed_WhenCalled_ShouldReturnView()
    {
        // Act
        var result = _controller.Failed();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }
}
