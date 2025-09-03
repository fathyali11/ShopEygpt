using FluentAssertions;
using Mapster;
using Web.Entites.Models;
using Xunit;

namespace Web.Entites.Mappings.Tests;

public class CartToOrderMappingConfigTests
{
    [Fact()]
    public void Register_WhenMapCartToOrder_ShouldReturnOrder()
    {
        // Arrange
        var config = new TypeAdapterConfig();
        var mappingConfig = new CartToOrderMappingConfig();
        mappingConfig.Register(config);
        var cart = new Cart
        {
            Id = 1,
            UserId = "user123",
            TotalPrice = 100.0m,
            CartItems = new List<CartItem>
            {
                new CartItem
                {
                    ProductId = 10,
                    ProductName = "Product A",
                    ImageName = "imageA.jpg",
                    Price = 50.0m,
                    Count = 1
                },
                new CartItem
                {
                    ProductId = 20,
                    ProductName = "Product B",
                    ImageName = "imageB.jpg",
                    Price = 25.0m,
                    Count = 2
                }
            }
        };

        // Act
        var order = cart.Adapt<Order>(config);

        // Assert
        order.Should().NotBeNull();
        order.Id.Should().Be(0); 
        order.TotalPrice.Should().Be(cart.TotalPrice);
        order.OrderItems.Should().HaveCount(2);
    }
    [Fact()]
    public void Register_WhenMapCartItemToOrderItem_ShouldReturnOrderItem()
    {
        // Arrange
        var config = new TypeAdapterConfig();
        var mappingConfig = new CartToOrderMappingConfig();
        mappingConfig.Register(config);
        var cartItem = new CartItem
        {
            ProductId = 10,
            ProductName = "Product A",
            ImageName = "imageA.jpg",
            Price = 50.0m,
            Count = 2
        };
        // Act
        var orderItem = cartItem.Adapt<OrderItem>(config);
        // Assert
        orderItem.Should().NotBeNull();
        orderItem.Id.Should().Be(0); 
        orderItem.ProductId.Should().Be(cartItem.ProductId);
        orderItem.ProductName.Should().Be(cartItem.ProductName);
        orderItem.ImageName.Should().Be(cartItem.ImageName);
        orderItem.UnitPrice.Should().Be(cartItem.Price);
        orderItem.Quantity.Should().Be(cartItem.Count);
    }
}