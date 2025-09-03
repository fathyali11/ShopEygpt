using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Web.Entites.Consts;
using Web.Entites.IRepositories;
using Web.Entites.Models;
using Web.Entites.ViewModels;
using Xunit;
namespace WearUp.Web.Controllers.Tests;
public class CategoryControllerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _controller = new CategoryController(_categoryRepositoryMock.Object);

        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _controller.TempData = tempData;
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new(){Id=1,Name="category1"},
            new(){Id=2,Name="category2"},
            new(){Id=3,Name="category3"},
            new(){Id=4,Name="category4"}
        };
        var paginatedCategories=PaginatedList<Category>.Create(categories,1,10);

        _categoryRepositoryMock
            .Setup(r => r.GetAllCategoriesAsync(It.IsAny<FilterRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedCategories);
        var request = new FilterRequest(null, null, null);
        // Act
        var result = await _controller.Index(request, CancellationToken.None);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().BeEquivalentTo(paginatedCategories);
    }

    [Fact]
    public async Task Create_WhenModelIsValidAndSuccess_ShouldRedirectToIndex()
    {
        // Arrange
        var model = new CreateCategoryVM("Shoes", new Mock<IFormFile>().Object);

        _categoryRepositoryMock
            .Setup(r => r.AddCategoryAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Create(model, CancellationToken.None);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        _controller.TempData.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_WhenModelIsInvalid_ShouldReturnSameView()
    {
        // Arrange
        var model = new CreateCategoryVM("", new Mock<IFormFile>().Object);
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Create(model, CancellationToken.None);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(model);
    }

    [Fact]
    public async Task Edit_WhenCategoryNotFound_ShouldRedirectToIndexWithError()
    {
        // Arrange
        _categoryRepositoryMock
            .Setup(r => r.GetCategoryAsync(It.IsAny<int>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync((EditCategoryVM?)null);
        // Act
        var result = await _controller.Edit(1);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        redirect.ControllerName.Should().Be("Category");
        _controller.TempData.Should().NotBeNull();
    }

    [Fact]
    public async Task Edit_WhenCategoryFound_ShouldGoToEditView()
    {
        // Arrange
        var category = new EditCategoryVM(1, "", "", new Mock<IFormFile>().Object);
        _categoryRepositoryMock
            .Setup(r => r.GetCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.Model.Should().Be(category);
    }

    [Fact]
    public async Task Delete_WhenSuccess_ShouldReturnJsonWithSuccess()
    {
        // Arrange
        _categoryRepositoryMock
            .Setup(r => r.DeleteCategoryAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = true, message = "Category deleted successfully." });
    }

    [Fact]
    public async Task Delete_WhenFails_ShouldReturnJsonWithError()
    {
        // Arrange
        _categoryRepositoryMock
            .Setup(r => r.DeleteCategoryAsync(1))
            .ReturnsAsync(new List<ValidationError>());

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = false, message = "Failed to delete category." });
    }
}
