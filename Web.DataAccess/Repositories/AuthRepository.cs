using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneOf;
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
        var userIsExist = await _userManager.FindByEmailAsync(request.Email);
        if (userIsExist is not null)
        {
            _logger.LogWarning("User already exists with email: {Email}", request.Email);
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
        _logger.LogInformation("User registration successful, email confirmation sent to: {Email}", request.Email);

        await _signInManager.SignInAsync(user, isPersistent: false);
        return true;
    }

}
