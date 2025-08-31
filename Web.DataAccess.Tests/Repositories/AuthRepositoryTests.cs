using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Web.DataAccess.Data;
using Web.Entites.IRepositories;
using Web.Entites.Models;
using Web.Entites.ViewModels.UsersVMs;
using Xunit;
namespace Web.DataAccess.Repositories.Tests;

public class AuthRepositoryTests
{
    [Fact()]
    public async Task RegisterAsync_WhenUserIsExits_ShouldReturnValidationError()
    {
        // arrange

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        context.Users.Add(new ApplicationUser
        {
            UserName = "t",
            Email = "t@email.com"
        });
        await context.SaveChangesAsync();

        var logger=new Mock<ILogger<AuthRepository>>().Object;

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);



        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!,context);
        var model = new RegisterVM
        {
            UserName = "t",
            Email = "t@email.com"
        };
        // act
        var result = await authRepository.RegisterAsync(model);

        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.ErrorMessage.Should().Be("This user has an email");
        error.PropertyName.Should().Be("Found");

    }
    [Fact()]
    public async Task RegisterAsync_WhenUserManagerCannotCreateUser_ShouldReturnValidationError()
    {
        // arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        context.Users.Add(new ApplicationUser
        {
            UserName = "t",
            Email = "t@email.com"
        });
        await context.SaveChangesAsync();

        var logger = new Mock<ILogger<AuthRepository>>().Object;

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());


        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!, context);
        var model = new RegisterVM
        {
            UserName = "t1",
            Email = "t1@email.com"
        };
        // act
        var result = await authRepository.RegisterAsync(model);

        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.ErrorMessage.Should().Be("Internal server error");
        error.PropertyName.Should().Be("ServerError");

    }
    [Fact()]
    public async Task RegisterAsync_WhenUserManagerCannotAssignUserToRole_ShouldReturnValidationError()
    {
        // arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        context.Users.Add(new ApplicationUser
        {
            UserName = "t",
            Email = "t@email.com"
        });
        await context.SaveChangesAsync();

        var logger = new Mock<ILogger<AuthRepository>>().Object;

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());


        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!, context);
        var model = new RegisterVM
        {
            UserName = "t1",
            Email = "t1@email.com"
        };
        // act
        var result = await authRepository.RegisterAsync(model);

        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.ErrorMessage.Should().Be("Internal server error");
        error.PropertyName.Should().Be("ServerError");

    }
    [Fact()]
    public async Task RegisterAsync_WhenUserCreatedSuccessfullt_ShouldReturnTrue()
    {
        // arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        context.Users.Add(new ApplicationUser
        {
            UserName = "t",
            Email = "t@email.com"
        });
        await context.SaveChangesAsync();

        var logger = new Mock<ILogger<AuthRepository>>().Object;

        var applicationUser = new Mock<IApplicaionUserRepository>();
        var emailRepository= new Mock<IEmailRepository>();
        var httpcontext = new Mock<IHttpContextAccessor>();

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
           .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
           .ReturnsAsync(string.Empty);

        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            emailRepository.Object, applicationUser.Object, httpcontext.Object, context);
        var model = new RegisterVM
        {
            UserName = "t1",
            Email = "t1@email.com"
        };
        // act
        var result = await authRepository.RegisterAsync(model);

        // assert
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeTrue();

    }
}