using FluentAssertions;
using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.OrderVMs;
using Xunit;

namespace Web.Entites.Mappings.Tests;

public class OrdersMappingConfigTests
{
    [Fact()]
    public void Register_WhenMapOrderToOrderDetailsReponseVM_ShouldReturnOrderDetailsReponseVM()
    {
        // arrange
        var config = new TypeAdapterConfig();
        var mapping = new OrdersMappingConfig();
        mapping.Register(config);
        var order = new Models.Order
        {
            Id = 1,
            User = new ApplicationUser { UserName = "testuser" },
            OrderItems = new List<Models.OrderItem>
            {
                new OrderItem { Id = 1, ProductName = "Product1", Quantity = 2,UnitPrice = 10 },
                new OrderItem { Id = 2, ProductName = "Product2", Quantity = 1,UnitPrice = 20 }
            }
        };

        // Act
        var orderDetailsVM = order.Adapt<OrderDetailsReponseVM>(config);

        // Assert
        orderDetailsVM.Should().NotBeNull();
        orderDetailsVM.Id.Should().Be(order.Id);
        orderDetailsVM.UserName.Should().Be(order.User.UserName);
        orderDetailsVM.OrderItems.Should().HaveCount(order.OrderItems.Count);
    }
    [Fact()]
    public void Register_WhenMapOrderToOrderResponseVM_ShouldReturnOrderResponseVM()
    {
        // arrange
        var config = new TypeAdapterConfig();
        var mapping = new OrdersMappingConfig();
        mapping.Register(config);
        var order = new Models.Order
        {
            Id = 1,
            User = new ApplicationUser { UserName = "testuser" },
        };
        // Act
        var orderResponseVM = order.Adapt<OrderResponseVM>(config);
        // Assert
        orderResponseVM.Should().NotBeNull();
        orderResponseVM.Id.Should().Be(order.Id);
        orderResponseVM.UserName.Should().Be(order.User.UserName);
    }
    [Fact()]
    public void Register_WhenMapOrderToOrderProfileVM_ShouldReturnOrderProfileVM()
    {
        // arrange
        var config = new TypeAdapterConfig();
        var mapping = new OrdersMappingConfig();
        mapping.Register(config);
        var order = new Models.Order
        {
            Id = 1,
            OrderItems = new List<Models.OrderItem>
            {
                new OrderItem { Id = 1, ProductName = "Product1", Quantity = 2,UnitPrice = 10 },
                new OrderItem { Id = 2, ProductName = "Product2", Quantity = 1,UnitPrice = 20 }
            }
        };
        // Act
        var orderProfileVM = order.Adapt<OrderProfileVM>(config);
        // Assert
        orderProfileVM.Should().NotBeNull();
        orderProfileVM.Id.Should().Be(order.Id);
        orderProfileVM.Items.Should().HaveCount(order.OrderItems.Count);
    }
    [Fact()]
    public void Register_WhenMapOrderItemToOrderItemDetailsVM_ShouldReturnOrderItemDetailsVM()
    {
        // arrange
        var config = new TypeAdapterConfig();
        var mapping = new OrdersMappingConfig();
        mapping.Register(config);
        var orderItem = new OrderItem
        {
            Id = 1,
            ProductName = "Product1",
            Quantity = 2,
            UnitPrice = 10
        };
        // Act
        var orderItemDetailsVM = orderItem.Adapt<OrderItemDetailsVM>(config);
        // Assert
        orderItemDetailsVM.Should().NotBeNull();
        orderItemDetailsVM.Id.Should().Be(orderItem.Id);
        orderItemDetailsVM.ProductName.Should().Be(orderItem.ProductName);
        orderItemDetailsVM.Quantity.Should().Be(orderItem.Quantity);
        orderItemDetailsVM.UnitPrice.Should().Be(orderItem.UnitPrice);
    }
    [Fact()]
    public void Register_WhenMapOrderItemToOrderItemProfileVM_ShouldReturnOrderItemProfileVM()
    {
        // arrange
        var config = new TypeAdapterConfig();
        var mapping = new OrdersMappingConfig();
        mapping.Register(config);
        var orderItem = new OrderItem
        {
            Id = 1,
            ProductName = "Product1",
            Quantity = 2,
            UnitPrice = 10
        };
        // Act
        var orderItemProfileVM = orderItem.Adapt<OrderItemProfileVM>(config);
        // Assert
        orderItemProfileVM.Should().NotBeNull();
        orderItemProfileVM.ProductName.Should().Be(orderItem.ProductName);
        orderItemProfileVM.Quantity.Should().Be(orderItem.Quantity);
        orderItemProfileVM.UnitPrice.Should().Be(orderItem.UnitPrice);
    }
}