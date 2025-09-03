using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Entites.IRepositories;
using Web.Entites.Models;
using Web.Entites.ViewModels;
using Web.Entites.ViewModels.UsersVMs;
using Xunit;
namespace WearUp.Web.Controllers.Tests;
public class AuthsControllerTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly AuthsController _controller;

    public AuthsControllerTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null);

        _controller = new AuthsController(_authRepositoryMock.Object, _signInManagerMock.Object);
    }

    [Fact]
    public async Task Register_WhenSuccess_ShouldRedirectToHome()
    {
        // Arrange
        var vm = new RegisterVM
        {
            FirstName = "John",
            LastName = "Doe",
            UserName = "john_doe",
            Email = "test@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        _authRepositoryMock
            .Setup(r => r.RegisterAsync(vm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Register(vm, CancellationToken.None);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        redirect.ControllerName.Should().Be("Home");
    }

    [Fact]
    public async Task Register_WhenError_ShouldReturnViewWithModelErrors()
    {
        // Arrange
        var vm = new RegisterVM
        {
            FirstName = "John",
            LastName = "Doe",
            UserName = "john_doe",
            Email = "invalid",
            Password = "pass",
            ConfirmPassword = "pass"
        };

        _authRepositoryMock
            .Setup(r => r.RegisterAsync(vm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(OneOf.OneOf<List<ValidationError>, bool>.FromT0(
                new List<ValidationError> { new("Email", "Invalid email") }));

        // Act
        var result = await _controller.Register(vm, CancellationToken.None);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(vm);
        _controller.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Logout_WhenCalled_ShouldRedirectToHome()
    {
        // Act
        var result = await _controller.Logout();

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        redirect.ControllerName.Should().Be("Home");
        _signInManagerMock.Verify(s => s.SignOutAsync(), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_WhenSuccess_ShouldReturnSuccessView()
    {
        // Arrange
        var vm = new ConfirmEmailVM("1", "token");
        _authRepositoryMock
            .Setup(r => r.ConfirmEmailAsync(vm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(OneOf.OneOf<List<ValidationError>, bool>.FromT1(true));

        // Act
        var result = await _controller.ConfirmEmail(vm, CancellationToken.None);

        // Assert
        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.ViewName.Should().Be("ConfirmEmailSuccess");
    }

    [Fact]
    public async Task ConfirmEmail_WhenError_ShouldReturnErrorView()
    {
        // Arrange
        var vm = new ConfirmEmailVM("1", "invalid");
        _authRepositoryMock
            .Setup(r => r.ConfirmEmailAsync(vm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(OneOf.OneOf<List<ValidationError>, bool>.FromT0(
                new List<ValidationError> { new("Token", "Invalid token") }));

        // Act
        var result = await _controller.ConfirmEmail(vm, CancellationToken.None);

        // Assert
        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.ViewName.Should().Be("ConfirmEmailError");
    }
}
