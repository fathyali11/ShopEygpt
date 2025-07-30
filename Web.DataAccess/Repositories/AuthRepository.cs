using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using OneOf;
using System.Net;
using System.Text;
using Web.Entites.Consts;
using Web.Entites.ViewModels.UsersVMs;

namespace Web.DataAccess.Repositories;
public class AuthRepository(
    ILogger<AuthRepository> _logger,
    UserManager<ApplicationUser>_userManager,
    SignInManager<ApplicationUser> _signInManager,
    IValidator<ConfirmEmailVM> _confirmEmailVMValidator,
    IValidator<ResendEmailConfirmationVM> _resendEmailConfirmationVMValidator,
    GeneralRepository _generalRepository,
    IEmailRepository _emailRepository,
    IUrlHelper _urlHelper,
    IHttpContextAccessor _httpContextAccessor) : IAuthRepository
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
        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        _logger.LogInformation("set user to customer role");


        _logger.LogInformation("User registration successful, email confirmation sent to: {Email}", request.Email);

        await SendEmailConfirmationAsync(user);

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
        if(!user.EmailConfirmed)
        {
            _logger.LogInformation("User with email {Email} not confirmed",user.Email);
            return new List<ValidationError> { new("NotConfirmed", "This email not confirmed") };
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

    public async Task<OneOf<List<ValidationError>, bool>> ConfirmEmailAsync(ConfirmEmailVM confirmEmailVM, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Confirming email for user ID: {UserId}", confirmEmailVM.UserId);

        var validationResult = await _generalRepository.ValidateRequest(_confirmEmailVMValidator, confirmEmailVM);
        if (validationResult is not null)
        {
            _logger.LogWarning("Validation failed for email confirmation: {Errors}", validationResult);
            return validationResult;
        }
        _logger.LogInformation("Validation passed for email confirmation");

        var user = await _userManager.FindByIdAsync(confirmEmailVM.UserId);
        if (user is null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", confirmEmailVM.UserId);
            return new List<ValidationError> { new ValidationError("NotFound", "User is not found") };
        }

        if (user.EmailConfirmed)
        {
            _logger.LogInformation("Email already confirmed for user ID: {UserId}", confirmEmailVM.UserId);
            return new List<ValidationError> { new ValidationError("Confirmed", "Email is confirmed") };
        }

        var decodedBytes = WebEncoders.Base64UrlDecode(confirmEmailVM.Token);
        var decodedToken = Encoding.UTF8.GetString(decodedBytes);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            _logger.LogError("Email confirmation failed for user ID: {UserId}, Errors: {Errors}", confirmEmailVM.UserId, result.Errors);
            return new List<ValidationError> { new ValidationError(error.Code, error.Description) };
        }

        _logger.LogInformation("Email confirmed successfully for user ID: {UserId}", confirmEmailVM.UserId);
        await _signInManager.SignInAsync(user,false);
        return true;
    }

    public async Task<OneOf<List<ValidationError>, bool>> ResendEmailConfirmationAsync(ResendEmailConfirmationVM resendEmailConfirmationVM, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resending email confirmation for email: {Email}", resendEmailConfirmationVM.Email);

        var validationResult = await _generalRepository.ValidateRequest(_resendEmailConfirmationVMValidator, resendEmailConfirmationVM);
        if (validationResult is not null)
        {
            _logger.LogWarning("Validation failed for resending email confirmation: {Errors}", validationResult);
            return validationResult;
        }
        _logger.LogInformation("Validation passed for resending email confirmation");

        var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.Email);
        if (user is null)
        {
            _logger.LogWarning("User not found with email: {Email}", resendEmailConfirmationVM.Email);
            return new List<ValidationError> { new ValidationError("NotFound", "User is not found") };
        }

        await SendEmailConfirmationAsync(user);
        _logger.LogInformation("Email confirmation resent successfully to: {Email}", resendEmailConfirmationVM.Email);
        return true;
    }





    private async Task SendEmailConfirmationAsync(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);

        var confirmationLink = _urlHelper.Action(
            action: "ConfirmEmail",
            controller: "Auths",
            values: new { userId = user.Id, token = encodedToken },
            protocol: _httpContextAccessor.HttpContext?.Request.Scheme
        );
        await _emailRepository.SendEmailAsync(user.Email!, "Email Confirmation", GetEmailBody(user.UserName!, confirmationLink!));
    }
    private string GetEmailBody(string userName,string confirmationLink)=>
        $@"
    <h2>Hello {userName},</h2>
    <p>Thank you for registering on our website.</p>
    <p>Please confirm your email by clicking the link below:</p>
    <a href='{confirmationLink}' style='
        display: inline-block;
        padding: 10px 20px;
        color: white;
        background-color: #28a745;
        text-decoration: none;
        border-radius: 5px;'>Confirm Email</a>
    <p>If you did not create this account, you can safely ignore this email.</p>
    <br/>
    <p>Thanks,<br/>The Team</p>";
}
