namespace Web.DataAccess.Repositories;
public class AuthRepository(
    ILogger<AuthRepository> _logger,
    UserManager<ApplicationUser>_userManager,
    SignInManager<ApplicationUser> _signInManager,
    IValidator<ConfirmEmailVM> _confirmEmailVMValidator,
    IValidator<ResendEmailConfirmationVM> _resendEmailConfirmationVMValidator,
    IValidator<ForgotPasswordVM> _forgetPasswordVMValidator,
    IValidator<ResetPasswordVM> _resetPasswordVMValidator,
    GeneralRepository _generalRepository,
    IEmailRepository _emailRepository,
    IApplicaionUserRepository _applicaionUserRepository,
    IHttpContextAccessor _httpContextAccessor,
    ApplicationDbContext _context) : IAuthRepository
{
    public async Task<OneOf<List<ValidationError>,bool>> RegisterAsync(RegisterVM request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Check if this email found");
        var userIsExist=await _context
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
        var addToRoleResult=await _userManager.AddToRoleAsync(user, UserRoles.Customer);
        if(!addToRoleResult.Succeeded)
        {
            _logger.LogError("Failed to assign user to role ");
            return new List<ValidationError> { new(PropertyName: "ServerError", "Internal server error") };
        }
        _logger.LogInformation("set user to customer role");


        _logger.LogInformation("User registration successful, email confirmation sent to: {Email}", request.Email);
        await _applicaionUserRepository.RemoveCacheKey(cancellationToken);
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

    public async Task<OneOf<List<ValidationError>, bool>> ForgetPasswordAsync(ForgotPasswordVM forgetPasswordVM, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing forget password request for email: {Email}", forgetPasswordVM.Email);

        var validationResult = await _generalRepository.ValidateRequest(_forgetPasswordVMValidator, forgetPasswordVM);
        if (validationResult is not null)
        {
            _logger.LogWarning("Validation failed for forget password: {Errors}", validationResult);
            return validationResult;
        }
        _logger.LogInformation("Validation passed for forget password");

        var user = await _userManager.FindByEmailAsync(forgetPasswordVM.Email);
        if (user is null)
        {
            _logger.LogWarning("User not found with email: {Email}", forgetPasswordVM.Email);
            return new List<ValidationError> { new ValidationError("NotFound", "User is not found") };
        }

        await SendForgotPasswordEmailAsync(user);
        _logger.LogInformation("Forget password email sent successfully to: {Email}", forgetPasswordVM.Email);
        return true;
    }

    public async Task<OneOf<List<ValidationError>, bool>> ResetPasswordAsync(ResetPasswordVM resetPasswordVM, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resetting password for user ID: {UserId}", resetPasswordVM.UserId);

        var validationResult = await _generalRepository.ValidateRequest(_resetPasswordVMValidator, resetPasswordVM);
        if (validationResult is not null)
        {
            _logger.LogWarning("Validation failed for reset password: {Errors}", validationResult);
            return validationResult;
        }
        _logger.LogInformation("Validation passed for reset password");

        var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
        if (user is null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", resetPasswordVM.UserId);
            return new List<ValidationError> { new ValidationError("NotFound", "User is not found") };
        }

        var decodedBytes = WebEncoders.Base64UrlDecode(resetPasswordVM.Token);
        var decodedToken = Encoding.UTF8.GetString(decodedBytes);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordVM.NewPassword);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            _logger.LogError("Password reset failed for user ID: {UserId}, Errors: {Errors}", resetPasswordVM.UserId, result.Errors);
            return new List<ValidationError> { new ValidationError(error.Code,error.Description) };
        }

        _logger.LogInformation("Password reset successful for user ID: {UserId}", resetPasswordVM.UserId);
        return true;
    }




    public ChallengeResult ExternalLogin(string provider, string redirectUrl)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<OneOf<ExternalLoginInfo?, bool>> ExternalLoginCallbackAsync(string? returnUrl, string? remoteError,CancellationToken cancellationToken=default)
    {
        returnUrl ??= "/";

        if (remoteError != null)
        {
            return false;
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return null as ExternalLoginInfo;

        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            return true;
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email != null)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!,
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)!
                };

                var createResult= await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var error = createResult.Errors.First();
                    _logger.LogError("User creation failed during external login for email: {Email}, Errors: {Errors}", email, createResult.Errors);
                    return null as ExternalLoginInfo;
                }

                var roleResult= await _userManager.AddToRoleAsync(user, UserRoles.Customer);
                if (!roleResult.Succeeded)
                {
                    var error = roleResult.Errors.First();
                    _logger.LogError("Adding user to role failed during external login for email: {Email}, Errors: {Errors}", email, roleResult.Errors);
                    return null as ExternalLoginInfo;
                }
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            await _applicaionUserRepository.RemoveCacheKey(cancellationToken);
            
            return true;
        }
        return null as ExternalLoginInfo;
    }
    

    private async Task SendEmailConfirmationAsync(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        _logger.LogInformation($"Sending decoded token: {encodedToken}");
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";

        var confirmationLink = $"{baseUrl}/Auths/ConfirmEmail?userId={user.Id}&token={encodedToken}";
        await _emailRepository.SendEmailAsync(user.Email!, "Email Confirmation", GetEmailConfirmationBody(user.UserName!, confirmationLink!));
    }
    private static string GetEmailConfirmationBody(string userName,string confirmationLink)=>
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

    private async Task SendForgotPasswordEmailAsync(ApplicationUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        _logger.LogInformation($"Sending password reset token: {encodedToken}");
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";

        var resetLink = $"{baseUrl}/Auths/ResetPassword?userId={user.Id}&token={encodedToken}";
        await _emailRepository.SendEmailAsync(user.Email!, "Reset Your Password", GetResetPasswordEmailBody(user.UserName!, resetLink!));
    }
    private static string GetResetPasswordEmailBody(string userName, string resetLink) =>
                $@"
        <h2>Hello {userName},</h2>
        <p>We received a request to reset your password.</p>
        <p>You can reset your password by clicking the link below:</p>
        <a href='{resetLink}' style='
            display: inline-block;
            padding: 10px 20px;
            color: white;
            background-color: #007bff;
            text-decoration: none;
            border-radius: 5px;'>Reset Password</a>
        <p>If you did not request a password reset, please ignore this email or contact support.</p>
        <br/>
        <p>Thanks,<br/>The Team</p>";

}
