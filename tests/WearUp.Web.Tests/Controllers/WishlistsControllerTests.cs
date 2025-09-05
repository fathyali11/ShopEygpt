using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using Web.Entites.IRepositories;
using Web.Entites.Models;
using Web.Entites.ViewModels.WishlistVMs;
using Xunit;

namespace WearUp.Web.Controllers.Tests;

public class WishlistsControllerTests
{
    private readonly WishlistsController _wishlistsController;
    private readonly Mock<IWishlistRepository> _wishlistRepositoryMock;

    public WishlistsControllerTests()
    {
        _wishlistRepositoryMock = new Mock<IWishlistRepository>();
        _wishlistsController = new WishlistsController(_wishlistRepositoryMock.Object);

        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _wishlistsController.TempData = tempData;
        // Fake User
        var claim = new Claim(ClaimTypes.NameIdentifier, "user");
        var claimIdentity = new ClaimsIdentity([claim], "mock");
        var user = new ClaimsPrincipal(claimIdentity);
        _wishlistsController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Toggle_WhenAjaxRequestAndValidItem_ShouldReturnJsonWithSuccess()
    {
        // Arrange
        var addWishlistItem = new AddWishlistItem(1, "", "", 1, true);
        _wishlistRepositoryMock.Setup(x => x.ToggelWishlistItemAsync(It.IsAny<string>(), It.IsAny<AddWishlistItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _wishlistsController.ControllerContext.HttpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

        // Act
        var result = await _wishlistsController.Toggle(addWishlistItem, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = true, isInWishlist = true });
    }

    [Fact]
    public async Task Toggle_WhenNonAjaxRequest_ShouldRedirectToHomeIndex()
    {
        // Arrange
        var addWishlistItem = new AddWishlistItem(1,"","",1,true);
        _wishlistRepositoryMock.Setup(x => x.ToggelWishlistItemAsync(It.IsAny<string>(), It.IsAny<AddWishlistItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _wishlistsController.Toggle(addWishlistItem, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
        redirectResult.ControllerName.Should().Be("Home");
    }

    [Fact]
    public async Task Count_WhenCalled_ShouldReturnJsonWithCount()
    {
        // Arrange
        _wishlistRepositoryMock.Setup(x => x.GetWishlistItemCountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await _wishlistsController.Count(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { count = 5 });
    }

    [Fact]
    public async Task Index_WhenCalled_ShouldReturnViewWithWishlistItems()
    {
        // Arrange
        var wishlistItems = new WishlistResponse(
            1,
            [
                new WishlistItem{ProductId=1,WishlistId=1},
                new WishlistItem{ProductId=2,WishlistId=1},
                ]
            );
        _wishlistRepositoryMock.Setup(x => x.GetWishlistItems(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlistItems);

        // Act
        var result = await _wishlistsController.Index(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().BeEquivalentTo(wishlistItems);
    }

    [Fact]
    public async Task Delete_WhenAjaxRequestAndValidItem_ShouldReturnSuccessJson()
    {
        // Arrange
        var deleteWishlistItem = new DeleteWishlistItem (1, 1);
        _wishlistRepositoryMock.Setup(x => x.DeleteWishlistItemAsync(It.IsAny<string>(), It.IsAny<DeleteWishlistItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);
        _wishlistsController.ControllerContext.HttpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

        // Act
        var result = await _wishlistsController.Delete(deleteWishlistItem, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = true, message = "The product was deleted successfully!", wishlistItemsCount = 4 });
    }

    [Fact]
    public async Task Delete_WhenAjaxRequestAndInvalidItem_ShouldReturnFailureJson()
    {
        // Arrange
        var deleteWishlistItem = new DeleteWishlistItem (1, 1);
        _wishlistRepositoryMock.Setup(x => x.DeleteWishlistItemAsync(It.IsAny<string>(), It.IsAny<DeleteWishlistItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(-1);
        _wishlistsController.ControllerContext.HttpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

        // Act
        var result = await _wishlistsController.Delete(deleteWishlistItem, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = false, message = "The product was not deleted." });
    }

    [Fact]
    public async Task Delete_WhenNonAjaxRequest_ShouldRedirectToHomeIndex()
    {
        // Arrange
        var deleteWishlistItem = new DeleteWishlistItem(1,1);
        _wishlistRepositoryMock.Setup(x => x.DeleteWishlistItemAsync(It.IsAny<string>(), It.IsAny<DeleteWishlistItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);

        // Act
        var result = await _wishlistsController.Delete(deleteWishlistItem, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
        redirectResult.ControllerName.Should().Be("Home");
    }

    [Fact]
    public async Task Clear_WhenAjaxRequest_ShouldReturnSuccessJson()
    {
        // Arrange
        _wishlistRepositoryMock.Setup(x => x.ClearWishlistAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _wishlistsController.ControllerContext.HttpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

        // Act
        var result = await _wishlistsController.Clear(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = true, message = "The wishlist was cleared successfully!" });
    }

    [Fact]
    public async Task Clear_WhenNonAjaxRequest_ShouldRedirectToHomeIndex()
    {
        // Arrange
        _wishlistRepositoryMock.Setup(x => x.ClearWishlistAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _wishlistsController.Clear(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
        redirectResult.ControllerName.Should().Be("Home");
    }
}