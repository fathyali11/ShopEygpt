using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Web.Entites.IRepositories;
using Web.Entites.Models;
using Web.Entites.ViewModels.CartItemVMs;
using Web.Entites.ViewModels.CartVMs;
using Xunit;
namespace WearUp.Web.Controllers.Tests;
public class CartControllerTests
{
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly CartController _controller;

    public CartControllerTests()
    {
        _cartRepositoryMock = new Mock<ICartRepository>();

        _controller = new CartController(_cartRepositoryMock.Object);

        // Fake User with Claims
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-123")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public async Task Index_WhenCalled_ShouldReturnViewWithCartItems()
    {
        // Arrange
        var cartResponse = new CartResponse
        {
            Id = 1,
            UserId = "userId",
            TotalPrice = 100,
            Items = [
                new CartItem{
                    CartId=1,
                    ProductId=1,
                    ProductName="Test1",
                    Price=50,
                    Count=1,
                },
                new CartItem{
                    CartId=1,
                    ProductId=2,
                    ProductName="Test2",
                    Price=50,
                    Count=1,
                }

                ]
        };
        _cartRepositoryMock
            .Setup(r => r.GetCartItemsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cartResponse);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().BeEquivalentTo(cartResponse);
    }

    [Fact]
    public async Task Add_WhenAjaxRequest_ShouldReturnJsonSuccess()
    {
        // Arrange
        var vm = new AddCartItemVM(1,"","",10,1);
        _controller.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

        _cartRepositoryMock
            .Setup(r => r.AddToCartAsync("user-123", vm, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Add(vm);

        // Assert
        var json = result.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeEquivalentTo(new { success = true, message = "The product was added successfully!" });
    }

    [Fact]
    public async Task Add_WhenNotAjaxRequest_ShouldReturnRedirectToIndex()
    {
        // Arrange
        var vm = new AddCartItemVM(1, "", "", 10, 1);

        _cartRepositoryMock
            .Setup(r => r.AddToCartAsync(It.IsAny<string>(), vm, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Add(vm);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = result as RedirectToActionResult;
        redirectResult!.ActionName.Should().Be("Index");
        redirectResult.ControllerName.Should().Be("Home");
    }

    [Fact]
    public async Task Count_WhenCalled_ShouldReturnJsonWithCount()
    {
        // Arrange
        _cartRepositoryMock
            .Setup(r => r.GetCartItemCountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await _controller.Count();

        // Assert
        var json = result.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeEquivalentTo(new { count = 5 });
    }

    [Fact]
    public async Task Increase_WhenCalled_ShouldReturnJsonWithUpdatedValues()
    {
        // Arrange
        var vm = new Delete_Increase_DecreaseCartItemVM (1,1);
       
        _cartRepositoryMock
            .Setup(r => r.IncreaseAsync(It.IsAny<string>(), vm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Delete_Increase_DecreaseCartItemResponse(3, 150.0m));

        // Act
        var result = await _controller.Increase(vm);

        // Assert
        var json = result.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeEquivalentTo(new { count = 3, totalPrice = 150.0m });
    }

    [Fact]
    public async Task Delete_WhenAjaxRequest_ShouldReturnJsonSuccess()
    {
        // Arrange
        var vm = new Delete_Increase_DecreaseCartItemVM (1, 1);
        _controller.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

        _cartRepositoryMock
            .Setup(r => r.DeleteCartItemAndReturnCartTotalPriceAsync("user-123", vm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(200.0m);

        // Act
        var result = await _controller.Delete(vm);

        // Assert
        var json = result.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeEquivalentTo(new
        {
            success = true,
            message = "The product was deleted successfully!",
            totalPrice = 200.0m
        });
    }

    [Fact]
    public async Task Clear_WhenNotAjax_ShouldRedirectToHome()
    {
        // Arrange
        var cartId = 10;
        _cartRepositoryMock
            .Setup(r => r.ClearCartAsync(It.IsAny<string>(), cartId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Clear(cartId);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        redirect.ControllerName.Should().Be("Home");
    }
}
