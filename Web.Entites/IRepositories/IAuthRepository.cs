using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using Web.Entites.ViewModels;
using Web.Entites.ViewModels.UsersVMs;

namespace Web.Entites.IRepositories;

public interface IAuthRepository
{
    Task<OneOf<List<ValidationError>, bool>> RegisterAsync(RegisterVM request, CancellationToken cancellationToken = default);
    Task<OneOf<List<ValidationError>, bool>> LoginAsync(LoginVM request, CancellationToken cancellationToken = default);

    Task<OneOf<List<ValidationError>, bool>> ConfirmEmailAsync(ConfirmEmailVM confirmEmailVM, CancellationToken cancellationToken = default);
    Task<OneOf<List<ValidationError>, bool>> ResendEmailConfirmationAsync(ResendEmailConfirmationVM resendEmailConfirmationVM, CancellationToken cancellationToken = default);

    Task<OneOf<List<ValidationError>, bool>> ForgetPasswordAsync(ForgotPasswordVM forgetPasswordVM, CancellationToken cancellationToken = default);
    Task<OneOf<List<ValidationError>, bool>> ResetPasswordAsync(ResetPasswordVM resetPasswordVM, CancellationToken cancellationToken = default);

    ChallengeResult ExternalLogin(string provider, string redirectUrl);
    Task<OneOf<ExternalLoginInfo?, bool>> ExternalLoginCallbackAsync(string? returnUrl, string? remoteError);

}