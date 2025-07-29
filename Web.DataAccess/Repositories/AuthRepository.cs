using Mapster;
using Microsoft.Extensions.Logging;
using OneOf;
using Web.Entites.Consts;
using Web.Entites.ViewModels.UsersVMs;

namespace Web.DataAccess.Repositories;
public class AuthRepository(
    ILogger<AuthRepository> _logger,
    UserManager<ApplicationUser>_userManager,
    SignInManager<ApplicationUser> _signInManager): IAuthRepository
{
    public async Task<OneOf<List<ValidationError>,bool>> RegisterAsync(RegisterVM request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Check if this email found");
        var userIsExist=await _userManager
            .Users.AnyAsync(u => u.Email == request.Email || u.UserName == request.UserName, cancellationToken);
        if (userIsExist)
        {
            _logger.LogWarning("User already exists with email: {Email} or user name :{UserName}", request.Email,request.UserName);
            return new List<ValidationError> { new("Found", "This user has an email") };
        }

        _logger.LogInformation("Creating new user with email: {Email}", request.Email);

        var user = request.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            _logger.LogError("User registration failed: {Errors}", result.Errors);
            return new List<ValidationError> { new(PropertyName: "ServerError", "Internal server error") };
        }
        //await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        if (string.IsNullOrEmpty(request.Role))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Customer);
            _logger.LogInformation("set user to customer role");
        }
        else
        {
            await _userManager.AddToRoleAsync(user, request.Role);
            _logger.LogInformation("set user to {Role}",request.Role);
        }


        _logger.LogInformation("User registration successful, email confirmation sent to: {Email}", request.Email);

        await _signInManager.SignInAsync(user, isPersistent: false);
        return true;
    }
    public async Task<OneOf<List<ValidationError>,bool>> LoginAsync(LoginVM request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user is null)
        {
            _logger.LogWarning("User not found with username: {Username}", request.UserName);
            return new List<ValidationError> { new("NotFound", "User not found") };
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Invalid password for user: {Username}", request.UserName);
            await _userManager.AccessFailedAsync(user);
            return new List<ValidationError> { new("InvalidPassword", "Invalid password") };
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        _logger.LogInformation("Login successful for user: {Username}", request.UserName);
        return true;
    }
}
