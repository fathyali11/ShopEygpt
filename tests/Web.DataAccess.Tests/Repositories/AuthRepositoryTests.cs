using Hangfire;

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
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);



        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!,context, backgroundJobs);
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
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;

        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!, context, backgroundJobs);
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

        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!, context, backgroundJobs);
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
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            emailRepository.Object, applicationUser.Object, httpcontext.Object, context, backgroundJobs);
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



    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldReturnValidationError()
    {
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var logger = new Mock<ILogger<AuthRepository>>().Object;

        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.LoginAsync(new LoginVM(null!,null!));


        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.ErrorMessage.Should().Be("User not found");
        error.PropertyName.Should().Be("NotFound");

    }
    [Fact]
    public async Task LoginAsync_WhenUserNotActive_ShouldReturnValidationError()
    {
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser
            {
                EmailConfirmed=true,
                IsActive=false
            });

        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.LoginAsync(new LoginVM(null!, null!));


        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("NotActive");
        error.ErrorMessage.Should().Be("This email not active");

    }
    [Fact]
    public async Task LoginAsync_WhenEmailNotConfirmed_ShouldReturnValidationError()
    {
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        var user = new ApplicationUser { EmailConfirmed = false };
        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.LoginAsync(new LoginVM(null!, null!));


        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("NotConfirmed");
        error.ErrorMessage.Should().Be("This email not confirmed");

    }
    [Fact]
    public async Task LoginAsync_WhenPasswordIsWrong_ShouldReturnValidationError()
    {
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        var user = new ApplicationUser { EmailConfirmed = true,IsActive=true };
        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(),It.IsAny<string>()))
           .ReturnsAsync(false);

        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var logger = new Mock<ILogger<AuthRepository>>().Object;

        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, null!, null!,
            null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.LoginAsync(new LoginVM(null!, null!));


        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("InvalidPassword");
        error.ErrorMessage.Should().Be("Invalid password");

    }
    [Fact]
    public async Task LoginAsync_WhenSuccessfullt_ShouldReturnTrue()
    {
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null);
        var user = new ApplicationUser { EmailConfirmed = true, IsActive = true };
        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
           .ReturnsAsync(true);


        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            signInManagerMock.Object, null!,
            null!, null!, null!, null!,
            null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.LoginAsync(new LoginVM(null!, null!));


        // assert
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeTrue();

    }



    [Fact]
    public async Task ConfirmEmailAsync_WhenRequestValidationFailed_ShouldReturnValidationError()
    {
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var confirmEmailValidator = new Mock<IValidator<ConfirmEmailVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ConfirmEmailVM>, ConfirmEmailVM>(It.IsAny<IValidator<ConfirmEmailVM>>(), It.IsAny<ConfirmEmailVM>()))
            .ReturnsAsync(new List<ValidationError>());
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, null!,
           null!, confirmEmailValidator,
           null!, null!, null!, generalRepository.Object,
           null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.ConfirmEmailAsync(new ConfirmEmailVM(null!, null!));

        // assert 
        result.IsT0.Should().BeTrue();


    }
    [Fact]
    public async Task ConfirmEmailAsync_WhenUserNotFound_ShouldReturnValidationError()
    {
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var confirmEmailValidator = new Mock<IValidator<ConfirmEmailVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ConfirmEmailVM>, ConfirmEmailVM>(It.IsAny<IValidator<ConfirmEmailVM>>(), It.IsAny<ConfirmEmailVM>()))
            .ReturnsAsync((List<ValidationError>?) null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
           null!, confirmEmailValidator,
           null!, null!, null!, generalRepository.Object,
           null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.ConfirmEmailAsync(new ConfirmEmailVM(null!, null!));

        // assert 
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("NotFound");
        error.ErrorMessage.Should().Be("User is not found");



    }
    [Fact]
    public async Task ConfirmEmailAsync_WhenEmailAlreadyConfirmed_ShouldReturnValidationError()
    {
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var confirmEmailValidator = new Mock<IValidator<ConfirmEmailVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ConfirmEmailVM>, ConfirmEmailVM>(It.IsAny<IValidator<ConfirmEmailVM>>(), It.IsAny<ConfirmEmailVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var user = new ApplicationUser
        {
            EmailConfirmed = true
        };
        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
           null!, confirmEmailValidator,
           null!, null!, null!, generalRepository.Object,
           null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.ConfirmEmailAsync(new ConfirmEmailVM(null!, null!));

        // assert 
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("Confirmed");
        error.ErrorMessage.Should().Be("Email is confirmed");

    }
    [Fact]
    public async Task ConfirmEmailAsync_WhenUserManagerCannotConfirmEmail_ShouldReturnValidationError()
    {
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var confirmEmailValidator = new Mock<IValidator<ConfirmEmailVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ConfirmEmailVM>, ConfirmEmailVM>(It.IsAny<IValidator<ConfirmEmailVM>>(), It.IsAny<ConfirmEmailVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var user = new ApplicationUser
        {
            EmailConfirmed = false
        };
        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        userManagerMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
           .ReturnsAsync(IdentityResult.Failed());
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
           null!, confirmEmailValidator,
           null!, null!, null!, generalRepository.Object,
           null!, null!, null!, null!, backgroundJobs);
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes("any-token"));
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("any-token"));

        

        // act
        var result = await authRepository.ConfirmEmailAsync(new ConfirmEmailVM("id",encodedToken));

        // assert 
        result.IsT0.Should().BeTrue();
        result.AsT0.Should().NotBeEmpty();

    }
    [Fact]
    public async Task ConfirmEmailAsync_WhenUserManagerConfirmEmailSuccessfully_ShouldReturnTrue()
    {
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var confirmEmailValidator = new Mock<IValidator<ConfirmEmailVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ConfirmEmailVM>, ConfirmEmailVM>(It.IsAny<IValidator<ConfirmEmailVM>>(), It.IsAny<ConfirmEmailVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null);

        var user = new ApplicationUser
        {
            EmailConfirmed = false
        };
        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        userManagerMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
           .ReturnsAsync(IdentityResult.Success);
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
           signInManagerMock.Object, confirmEmailValidator,
           null!, null!, null!, generalRepository.Object,
           null!, null!, null!, null!, backgroundJobs);
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes("any-token"));
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("any-token"));



        // act
        var result = await authRepository.ConfirmEmailAsync(new ConfirmEmailVM("id", encodedToken));

        // assert 
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeTrue();

    }


    [Fact]
    public async Task ResendEmailConfirmationAsync__WhenRequestValidationFailed_ShouldReturnValidationError()
    {
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var resendEmailConfirmation = new Mock<IValidator<ResendEmailConfirmationVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ResendEmailConfirmationVM>, ResendEmailConfirmationVM>(It.IsAny<IValidator<ResendEmailConfirmationVM>>(), It.IsAny<ResendEmailConfirmationVM>()))
            .ReturnsAsync(new List<ValidationError>());
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, null!,
           null!, null!,
           resendEmailConfirmation, null!, null!, generalRepository.Object,
           null!, null!, null!, null!, backgroundJobs);


        // act
        var result = await authRepository.ResendEmailConfirmationAsync(new ResendEmailConfirmationVM(string.Empty));

        // assert

        result.IsT0.Should().BeTrue();
    }
    [Fact]
    public async Task ResendEmailConfirmationAsync__WhenUserNotFound_ShouldReturnValidationError()
    {
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var resendEmailConfirmation = new Mock<IValidator<ResendEmailConfirmationVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ResendEmailConfirmationVM>, ResendEmailConfirmationVM>(It.IsAny<IValidator<ResendEmailConfirmationVM>>(), It.IsAny<ResendEmailConfirmationVM>()))
            .ReturnsAsync( (List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
           null!, null!,
           resendEmailConfirmation, null!, null!, generalRepository.Object,
           null!, null!, null!, null!, backgroundJobs);


        // act
        var result = await authRepository.ResendEmailConfirmationAsync(new ResendEmailConfirmationVM(string.Empty));

        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("NotFound");
        error.ErrorMessage.Should().Be("User is not found");

    }
    [Fact]
    public async Task ResendEmailConfirmationAsync__WhenEmailResentSuccessfully_ShouldReturnTrue()
    {
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var httpContextAccessor = new Mock<IHttpContextAccessor>().Object;
        var emailRepository = new Mock<IEmailRepository>().Object;

        var resendEmailConfirmation = new Mock<IValidator<ResendEmailConfirmationVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ResendEmailConfirmationVM>, ResendEmailConfirmationVM>(It.IsAny<IValidator<ResendEmailConfirmationVM>>(), It.IsAny<ResendEmailConfirmationVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser());

        userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(string.Empty);
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
           null!, null!,
           resendEmailConfirmation, null!, null!, generalRepository.Object,
           emailRepository, null!, httpContextAccessor, null!, backgroundJobs);


        // act
        var result = await authRepository.ResendEmailConfirmationAsync(new ResendEmailConfirmationVM(string.Empty));

        // assert
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeTrue();

    }



    [Fact]
    public async Task ForgetPasswordAsync_WhenRequestValidationFails_ShouldReturnValidationError()
    {
        // arrange
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var forgetPasswordValidator = new Mock<IValidator<ForgotPasswordVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ForgotPasswordVM>, ForgotPasswordVM>(
            It.IsAny<IValidator<ForgotPasswordVM>>(), It.IsAny<ForgotPasswordVM>()))
            .ReturnsAsync(new List<ValidationError>());
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, null!,
            null!, null!,
            null!, forgetPasswordValidator, null!, generalRepository.Object,
            null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.ForgetPasswordAsync(new ForgotPasswordVM(null!));

        // assert
        result.IsT0.Should().BeTrue();
    }
    [Fact]
    public async Task ForgetPasswordAsync_WhenUserNotFound_ShouldReturnValidationError()
    {
        // arrange
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var forgetPasswordValidator = new Mock<IValidator<ForgotPasswordVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ForgotPasswordVM>, ForgotPasswordVM>(
            It.IsAny<IValidator<ForgotPasswordVM>>(), It.IsAny<ForgotPasswordVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, forgetPasswordValidator, null!, generalRepository.Object,
            null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.ForgetPasswordAsync(new ForgotPasswordVM("test@email.com"));

        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("NotFound");
        error.ErrorMessage.Should().Be("User is not found");
    }
    [Fact]
    public async Task ForgetPasswordAsync_WhenSuccessful_ShouldReturnTrue()
    {
        // arrange
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var forgetPasswordValidator = new Mock<IValidator<ForgotPasswordVM>>().Object;
        var httpContextAccessor = new Mock<IHttpContextAccessor>().Object;
        var emailRepository = new Mock<IEmailRepository>().Object;

        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ForgotPasswordVM>, ForgotPasswordVM>(
            It.IsAny<IValidator<ForgotPasswordVM>>(), It.IsAny<ForgotPasswordVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser());

        userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("reset-token");
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, forgetPasswordValidator, null!, generalRepository.Object,
            emailRepository, null!, httpContextAccessor, null!, backgroundJobs);

        // act
        var result = await authRepository.ForgetPasswordAsync(new ForgotPasswordVM("test@email.com"));

        // assert
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeTrue();
    }


    [Fact]
    public async Task ResetPasswordAsync_WhenRequestValidationFails_ShouldReturnValidationError()
    {
        // arrange
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var resetPasswordValidator = new Mock<IValidator<ResetPasswordVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ResetPasswordVM>, ResetPasswordVM>(
            It.IsAny<IValidator<ResetPasswordVM>>(), It.IsAny<ResetPasswordVM>()))
            .ReturnsAsync(new List<ValidationError>());
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, null!,
            null!, null!,
            null!, null!, resetPasswordValidator, generalRepository.Object,
            null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.ResetPasswordAsync(new ResetPasswordVM("userId", "token", "newPassword"));

        // assert
        result.IsT0.Should().BeTrue();
    }
    [Fact]
    public async Task ResetPasswordAsync_WhenUserNotFound_ShouldReturnValidationError()
    {
        // arrange
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var resetPasswordValidator = new Mock<IValidator<ResetPasswordVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ResetPasswordVM>, ResetPasswordVM>(
            It.IsAny<IValidator<ResetPasswordVM>>(), It.IsAny<ResetPasswordVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, resetPasswordValidator, generalRepository.Object,
            null!, null!, null!, null!, backgroundJobs);

        // act
        var result = await authRepository.ResetPasswordAsync(new ResetPasswordVM("userId", "token", "newPassword"));

        // assert
        result.IsT0.Should().BeTrue();
        var error = result.AsT0.First();
        error.PropertyName.Should().Be("NotFound");
        error.ErrorMessage.Should().Be("User is not found");
    }
    [Fact]
    public async Task ResetPasswordAsync_WhenPasswordResetFails_ShouldReturnValidationError()
    {
        // arrange
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var resetPasswordValidator = new Mock<IValidator<ResetPasswordVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ResetPasswordVM>, ResetPasswordVM>(
            It.IsAny<IValidator<ResetPasswordVM>>(), It.IsAny<ResetPasswordVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var user = new ApplicationUser();
        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        userManagerMock.Setup(x => x.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "InvalidToken", Description = "Invalid token" }));
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, resetPasswordValidator, generalRepository.Object,
            null!, null!, null!, null!, backgroundJobs);
        var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("any-token"));
        // act
        var result = await authRepository.ResetPasswordAsync(new ResetPasswordVM("", "id", token));

        // assert
        result.IsT0.Should().BeTrue();
    }
    [Fact]
    public async Task ResetPasswordAsync_WhenSuccessful_ShouldReturnTrue()
    {
        // arrange
        var logger = new Mock<ILogger<AuthRepository>>().Object;
        var resetPasswordValidator = new Mock<IValidator<ResetPasswordVM>>().Object;
        var generalRepository = new Mock<IGeneralRepository>();
        generalRepository.Setup(x => x.ValidateRequest<IValidator<ResetPasswordVM>, ResetPasswordVM>(
            It.IsAny<IValidator<ResetPasswordVM>>(), It.IsAny<ResetPasswordVM>()))
            .ReturnsAsync((List<ValidationError>?)null);

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var user = new ApplicationUser();
        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        userManagerMock.Setup(x => x.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var backgroundJobs = new Mock<IBackgroundJobsRepository>().Object;
        var authRepository = new AuthRepository(logger, userManagerMock.Object,
            null!, null!,
            null!, null!, resetPasswordValidator, generalRepository.Object,
            null!, null!, null!, null!, backgroundJobs);
        var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("any-token"));
        // act
        var result = await authRepository.ResetPasswordAsync(new ResetPasswordVM("", "id", token));

        // assert
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeTrue();
    }

}