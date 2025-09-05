namespace Web.Entites.Mappings.Tests;

public class UserMappingTests
{
    [Fact]
    public void Register_WhenMapRegisterVMToApplicationUser_ShouldReturnApplicationUser()
    {
        // arrange
        var mapping = new UserMapping();
        var config = new TypeAdapterConfig();
        mapping.Register(config);
        var registerVM = new RegisterVM
        {
            UserName = "testuser",
            Email = "test@email.com",
            FirstName = "Test",
            LastName = "Test",
            Password = "Password123!"
        };

        // act
        var applicationUser = registerVM.Adapt<ApplicationUser>(config);
        // assert
        applicationUser.Should().NotBeNull();
        applicationUser.UserName.Should().Be(registerVM.UserName);
        applicationUser.Email.Should().Be(registerVM.Email);
        applicationUser.FirstName.Should().Be(registerVM.FirstName);
        applicationUser.LastName.Should().Be(registerVM.LastName);


    }

    [Fact]
    public void Register_WhenMapApplicationUserToUserResponseForAdmin_ShouldReturnUserResponseForAdmin()
    {
        // arrange
        var mapping = new UserMapping();
        var config = new TypeAdapterConfig();
        mapping.Register(config);
        var applicationUser = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@email.com",
            FirstName = "Test",
            LastName = "Test"
        };

        // act
        var userResponseForAdmin = applicationUser.Adapt<UserResponseForAdmin>(config);

        // assert
        userResponseForAdmin.Should().NotBeNull();
        userResponseForAdmin.UserName.Should().Be(applicationUser.UserName);
        userResponseForAdmin.Email.Should().Be(applicationUser.Email);
        userResponseForAdmin.FirstName.Should().Be(applicationUser.FirstName);
        userResponseForAdmin.LastName.Should().Be(applicationUser.LastName);
    }

    [Fact]
    public void Register_WhenMapEditUserVMToApplicationUser_ShouldReturnApplicationUser()
    {
        // arrange
        var mapping = new UserMapping();
        var config = new TypeAdapterConfig();
        mapping.Register(config);

        var editUserVM = new EditUserVM
        {
            Id = "1",
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Email = "updateEmail@email.com",
            UserName = "updatedUserName",
            Role = "Admin"
        };

        // act
        var applicationUser = editUserVM.Adapt<ApplicationUser>(config);
        // assert
        applicationUser.Should().NotBeNull();
        applicationUser.Id.Should().NotBe(editUserVM.Id); 
        applicationUser.UserName.Should().Be(editUserVM.UserName);
        applicationUser.Email.Should().Be(editUserVM.Email);
        applicationUser.FirstName.Should().Be(editUserVM.FirstName);
        applicationUser.LastName.Should().Be(editUserVM.LastName);

    }

    [Fact]
    public void Register_WhenMapCreateUserVMToApplicationUser_ShouldReturnApplicationUser()
    {
        // arrange
        var mapping = new UserMapping();
        var config = new TypeAdapterConfig();
        mapping.Register(config);
        var createUserVM = new CreateUserVM
        {
            UserName = "testuser",
            Email = "test@email.com",
            FirstName = "Test",
            LastName = "Test",
            Password = "Password123!",
            Role = "Admin"
        };
        // act
        var applicationUser = createUserVM.Adapt<ApplicationUser>(config);
        // assert
        applicationUser.Should().NotBeNull();
        applicationUser.UserName.Should().Be(createUserVM.UserName);
        applicationUser.Email.Should().Be(createUserVM.Email);
        applicationUser.FirstName.Should().Be(createUserVM.FirstName);
        applicationUser.LastName.Should().Be(createUserVM.LastName);
        }
}
