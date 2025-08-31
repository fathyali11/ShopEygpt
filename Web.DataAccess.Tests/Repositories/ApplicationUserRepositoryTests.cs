using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Web.DataAccess.Data;
using Web.DataAccess.Tests;
using Web.Entites.Consts;
using Web.Entites.Models;
using Web.Entites.ViewModels.UsersVMs;
using Xunit;

namespace Web.DataAccess.Repositories.Tests;

public class ApplicationUserRepositoryTests
{
    [Fact]
    public async Task GetAllUsersAsync_WhenUsersExist_ShouldReturnPaginatedList()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var roles = new List<IdentityRole>
        {
            new() { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new() { Id = "2", Name = "User", NormalizedName = "USER" }
        };
        await context.Roles.AddRangeAsync(roles);

        var users = new List<ApplicationUser>
        {
            new() { Id = "u1", UserName = "Alice", Email = "alice@email.com", FirstName="Alice", LastName="Smith", IsActive=true },
            new() { Id = "u2", UserName = "Bob", Email = "bob@email.com", FirstName="Bob", LastName="Brown", IsActive=true }
        };
        await context.Users.AddRangeAsync(users);

        var userRoles = new List<IdentityUserRole<string>>
        {
            new() { UserId = "u1", RoleId = "1" },
            new() { UserId = "u2", RoleId = "2" }    
        };
        await context.UserRoles.AddRangeAsync(userRoles);
        await context.SaveChangesAsync();

        var hybridCache = new FakeHybridCache();


        var repo = new ApplicationUserRepository(context, null!, hybridCache);

        var request = new FilterRequest(
            null,
            null,
            null,
            1
            );

        // Act
        var result = await repo.GetAllUsersAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.First().UserName.Should().Be("Alice"); 
        result.Items.Last().UserName.Should().Be("Bob");
    }



    [Fact]
    public async Task CreateUserAsync_WhenUserIsExists_ShouldReturnValidationError()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique db for each test
            .Options;

        using var context = new ApplicationDbContext(options);

        var existingUser = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var hybridCacheMock = new Mock<HybridCache>();

        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);

        var model = new CreateUserVM
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            Role = "User"
        };

        // Act
        var result = await repository.CreateUserAsync(model);

        // Assert
        result.IsT1.Should().BeTrue();
        var error = result.AsT1;
        error.Should().NotBeNull();
        error.PropertyName.Should().Be("User Found");
        error.ErrorMessage.Should().Be("UserName or Email already exists.");
    }

    [Fact]
    public async Task CreateUserAsync_WhenUserManagerFailsToCreateUser_ShouldReturnValidationError()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        var hybridCacheMock = new Mock<HybridCache>();
        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);
        var model = new CreateUserVM
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            Role = "User"
        };
        // Act
        var result = await repository.CreateUserAsync(model);

        // Assert
        result.IsT1.Should().BeTrue();
        var error = result.AsT1;
        error.Should().NotBeNull();
        error.PropertyName.Should().Be("Create User Failed");
        error.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUserAsync_WhenUserManagerFailsToAssignRole_ShouldReturnValidationError()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        var hybridCacheMock = new Mock<HybridCache>();
        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);
        var model = new CreateUserVM
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            Role = "User"
        };

        // Act
        var result = await repository.CreateUserAsync(model);

        // Assert
        result.IsT1.Should().BeTrue();
        var error = result.AsT1;
        error.Should().NotBeNull();
        error.PropertyName.Should().Be("Assign Role Failed");
        error.ErrorMessage.Should().NotBeNull();

    }

    [Fact]
    public async Task CreateUserAsync_WhenSuccessful_ShouldReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
              .Returns<ApplicationUser, string>(async (user, password) =>
              {
                  context.Users.Add(user);
                  await context.SaveChangesAsync();
                  return IdentityResult.Success;
              });


        userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var hybridCacheMock = new Mock<HybridCache>();


        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);
        var model = new CreateUserVM
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            Role = "User"
        };

        // Act
        var result = await repository.CreateUserAsync(model);
        // Assert
        result.IsT0.Should().BeTrue();
        var isSuccess = result.AsT0;
        isSuccess.Should().BeTrue();
        var createdUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "testuser");
        createdUser.Should().NotBeNull();

    }

    [Fact()]
    public async Task UpdateUserByAdminAsync_WhenUserNotFound_ShouldReturnFalse()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var hybridCacheMock = new Mock<HybridCache>();

        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);

        var result = await repository.UpdateUserByAdminAsync(new EditUserVM());

        result.Should().BeFalse();
    }

    [Fact()]
    public async Task UpdateUserByAdminAsync_WhenRoleNotFound_ShouldReturnFalse()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var hybridCacheMock = new Mock<HybridCache>();

        var user = new ApplicationUser
        {
            Id = "1",
            UserName = "testuser",
            Email = "test@email.com"
        };

        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);




        // act
        var result = await repository.UpdateUserByAdminAsync(new EditUserVM { Id = "1" });

        result.Should().BeFalse();
    }
    [Fact()]
    public async Task UpdateUserByAdminAsync_WhenSuccessful_ShouldReturnTrue()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var hybridCacheMock = new Mock<HybridCache>();
        var user = new ApplicationUser
        {
            Id = "1",
            UserName = "testuser",
            Email = "test@email.com"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var roles = new List<IdentityRole>
        {
            new() { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new() { Id = "2", Name = "User", NormalizedName = "USER" }
        };
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();

        var userRole = new IdentityUserRole<string>
        {
            RoleId = "1",
            UserId = "1"
        };
        context.UserRoles.Add(userRole);
        await context.SaveChangesAsync();

        var model = new EditUserVM
        {
            Id = "1",
            UserName = "updateduser",
            Email = "test@email.com",
            Role = "User"
        };
        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);

        // act
        var result = await repository.UpdateUserByAdminAsync(model);

        // assert

        result.Should().BeTrue();
        var updatedUserWithRole = await (from u in context.Users
                                         join ur in context.UserRoles on u.Id equals ur.UserId
                                         join r in context.Roles on ur.RoleId equals r.Id
                                         where u.Id == "1"
                                         select new { User = u, Role = r.Name }).FirstOrDefaultAsync();
        updatedUserWithRole.Role.Should().Be("User");
        updatedUserWithRole.User.UserName.Should().Be("updateduser");
    }


    [Fact()]
    public async Task GetUserProfileAsync_WhenUserFound_ShouldReturnUserProfileVM()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var hybridCacheMock = new Mock<HybridCache>();
        var user = new ApplicationUser
        {
            Id = "1",
            UserName = "testuser",
            Email = "test@email.com"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);

        // act
        var result = await repository.GetUserProfileAsync("1");

        // assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@email.com");
        result.UserName.Should().Be("testuser");

    }


    [Fact()]
    public async Task ChangeUserPasswordAsync_WhenUserNotFound_ShouldReturnFalse()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        var hybridCacheMock = new Mock<HybridCache>();

        userManagerMock.Setup(x=>x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
        
        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);
        var model = new ChangePasswordVM
        {
            CurrentPassword = "OldPassword",
            NewPassword = "NewPassword",
            ConfirmNewPassword = "NewPassword"
        };
        // act
        var result = await repository.ChangeUserPasswordAsync("1", model);

        // assert

        result.Should().BeFalse();

    }
    [Fact()]
    public async Task ChangeUserPasswordAsync_WhenCurrentPasswordInValid_ShouldReturnFalse()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        var hybridCacheMock = new Mock<HybridCache>();

        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(It.IsAny<ApplicationUser>());

        userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);
        var model = new ChangePasswordVM
        {
            CurrentPassword = "OldPassword",
            NewPassword = "NewPassword",
            ConfirmNewPassword = "NewPassword"
        };
        // act
        var result = await repository.ChangeUserPasswordAsync("1", model);

        // assert

        result.Should().BeFalse();

    }
    [Fact()]
    public async Task ChangeUserPasswordAsync_WhenModelIsValid_ShouldReturnTrue()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new ApplicationDbContext(options);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        var hybridCacheMock = new Mock<HybridCache>();

        var fakeUser = new ApplicationUser
        {
            Id = "123",
            UserName = "TestUser",
            Email = "test@test.com"
        };

        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(fakeUser);


        userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var repository = new ApplicationUserRepository(context, userManagerMock.Object, hybridCacheMock.Object);
        var model = new ChangePasswordVM
        {
            CurrentPassword = "OldPassword",
            NewPassword = "NewPassword",
            ConfirmNewPassword = "NewPassword"
        };
        // act
        var result = await repository.ChangeUserPasswordAsync("1", model);

        // assert

        result.Should().BeTrue();

    }


}
