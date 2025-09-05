using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using Web.Entites.Consts;
using Web.Entites.IRepositories;
using Web.Entites.ViewModels;
using Web.Entites.ViewModels.UsersVMs;
using Xunit;

namespace WearUp.Web.Controllers.Tests;

public class UsersControllerTests
{
    private readonly UsersController _usersController;
    private readonly Mock<IApplicaionUserRepository> _userRepositoryMock;

    public UsersControllerTests()
    {
        _userRepositoryMock = new Mock<IApplicaionUserRepository>();
        _usersController = new UsersController(_userRepositoryMock.Object);

        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _usersController.TempData = tempData;

        // Fake User
        var claim = new Claim(ClaimTypes.NameIdentifier, "user");
        var claimIdentity = new ClaimsIdentity([claim], "mock");
        var user = new ClaimsPrincipal(claimIdentity);
        _usersController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Index_WhenCallIt_ShouldReturnViewWithUsers()
    {
        // Arrange
        var users = new List<UserResponseForAdmin>
        {
            new() { Id = "1", UserName = "user1" },
            new() { Id = "2", UserName = "user2" }
        };
        var paginatedUsers = PaginatedList<UserResponseForAdmin>.Create(users, 1, 10);
        var request = new FilterRequest(null!, null!, null!);
        _userRepositoryMock.Setup(x => x.GetAllUsersAsync(It.IsAny<FilterRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedUsers);

        // Act
        var result = await _usersController.Index(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().BeEquivalentTo(paginatedUsers);
    }

    [Fact]
    public async Task Edit_WhenValidId_ShouldReturnViewWithUser()
    {
        // Arrange
        var user = new EditUserVM { Id = "1", UserName = "user1" };
        _userRepositoryMock.Setup(x => x.GetUserForEditByAdminAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _usersController.Edit("1");

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().Be(user);
    }

    [Fact]
    public async Task Edit_WhenPostValidModel_ShouldRedirectToIndex()
    {
        // Arrange
        var model = new EditUserVM { Id = "1", UserName = "user1" };
        _userRepositoryMock.Setup(x => x.UpdateUserByAdminAsync(It.IsAny<EditUserVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _usersController.Edit(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
    }

    [Fact]
    public async Task Edit_WhenPostInvalidModel_ShouldReturnViewWithErrors()
    {
        // Arrange
        var model = new EditUserVM();
        _userRepositoryMock.Setup(x => x.UpdateUserByAdminAsync(It.IsAny<EditUserVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _usersController.ModelState.AddModelError("UserName", "UserName is required");

        // Act
        var result = await _usersController.Edit(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(model);
    }

    [Fact]
    public async Task Toggle_WhenValidId_ShouldReturnSuccessJson()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.ToggleUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _usersController.Toggle("1", CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = true, message = "user toggled successfully" });
    }

    [Fact]
    public async Task Toggle_WhenInvalidId_ShouldReturnFailureJson()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.ToggleUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _usersController.Toggle("1", CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = false, message = "user doesn't toggle" });
    }

    [Fact]
    public async Task Delete_WhenValidId_ShouldReturnSuccessJson()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.DeleteUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _usersController.Delete("1", CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = true, message = "user deleted successfully" });
    }

    [Fact]
    public async Task Delete_WhenInvalidId_ShouldReturnFailureJson()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.DeleteUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _usersController.Delete("1", CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = false, message = "user doesn't delete" });
    }

    [Fact]
    public void Create_WhenGetRequest_ShouldReturnView()
    {
        // Act
        var result = _usersController.Create();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Create_WhenPostValidModel_ShouldRedirectToIndex()
    {
        // Arrange
        var model = new CreateUserVM { UserName = "user1" };
        _userRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _usersController.Create(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
    }

    [Fact]
    public async Task Create_WhenPostInvalidModel_ShouldReturnViewWithErrors()
    {
        // Arrange
        var model = new CreateUserVM();
        var error = new ValidationError("UserName", "UserName is required");
        _userRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(error);
        _usersController.ModelState.AddModelError("UserName", "UserName is required");

        // Act
        var result = await _usersController.Create(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(model);
    }

    [Fact]
    public async Task Profile_WhenCallIt_ShouldReturnViewWithUserProfile()
    {
        // Arrange
        var userProfile = new UserProfileVM ();
        _userRepositoryMock.Setup(x => x.GetUserProfileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userProfile);

        // Act
        var result = await _usersController.Profile(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().Be(userProfile);
    }

    [Fact]
    public async Task EditProfile_WhenGetRequest_ShouldReturnViewWithEditProfile()
    {
        // Arrange
        var userProfile = new UserProfileVM ();
        var editProfile = new EditUserProfileVM ();
        _userRepositoryMock.Setup(x => x.GetUserProfileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userProfile);

        // Act
        var result = await _usersController.EditProfile(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().BeOfType<EditUserProfileVM>();
    }

    [Fact]
    public async Task EditProfile_WhenPostValidModel_ShouldRedirectToProfile()
    {
        // Arrange
        var model = new EditUserProfileVM();
        _userRepositoryMock.Setup(x => x.UpdateUserProfileAsync(It.IsAny<string>(), It.IsAny<EditUserProfileVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _usersController.EditProfile(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Profile");
    }

    [Fact]
    public async Task EditProfile_WhenPostInvalidModel_ShouldReturnViewWithErrors()
    {
        // Arrange
        var model = new EditUserProfileVM();
        _userRepositoryMock.Setup(x => x.UpdateUserProfileAsync(It.IsAny<string>(), It.IsAny<EditUserProfileVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _usersController.ModelState.AddModelError("UserName", "UserName is required");

        // Act
        var result = await _usersController.EditProfile(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(model);
    }

    [Fact]
    public void ChangePassword_WhenGetRequest_ShouldReturnView()
    {
        // Act
        var result = _usersController.ChangePassword();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task ChangePassword_WhenPostValidModel_ShouldRedirectToProfile()
    {
        // Arrange
        var model = new ChangePasswordVM { CurrentPassword = "old", NewPassword = "new" };
        _userRepositoryMock.Setup(x => x.ChangeUserPasswordAsync(It.IsAny<string>(), It.IsAny<ChangePasswordVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _usersController.ChangePassword(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Profile");
    }

    [Fact]
    public async Task ChangePassword_WhenPostInvalidModel_ShouldReturnViewWithErrors()
    {
        // Arrange
        var model = new ChangePasswordVM();
        _userRepositoryMock.Setup(x => x.ChangeUserPasswordAsync(It.IsAny<string>(), It.IsAny<ChangePasswordVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _usersController.ModelState.AddModelError("NewPassword", "NewPassword is required");

        // Act
        var result = await _usersController.ChangePassword(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(model);
    }
}