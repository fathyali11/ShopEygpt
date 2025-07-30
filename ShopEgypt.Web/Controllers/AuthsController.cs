using Microsoft.AspNetCore.Identity;
using Web.Entites.ViewModels.UsersVMs;

namespace ShopEgypt.Web.Controllers;
public class AuthsController(IAuthRepository _authRepository,
    SignInManager<ApplicationUser> _signInManager) : Controller
{
    [HttpGet]
    public IActionResult Register() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM registerVM,CancellationToken cancellationToken)
    {
        var result = await _authRepository.RegisterAsync(registerVM, cancellationToken);
        if (result.IsT1)
            return RedirectToAction("Index", "Home");

        var errors = result.AsT0;
        foreach (var item in errors)
            ModelState.AddModelError(item.PropertyName, item.ErrorMessage);

        return View(registerVM);

    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        var result = await _authRepository.LoginAsync(loginVM, cancellationToken);

        if (result.IsT1)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        var errors = result.AsT0;
        foreach (var item in errors)
            ModelState.AddModelError(item.PropertyName, item.ErrorMessage);

        ViewData["ReturnUrl"] = returnUrl;
        return View(loginVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM confirmEmailVM, CancellationToken cancellationToken)
    {
        var result = await _authRepository.ConfirmEmailAsync(confirmEmailVM, cancellationToken);

        return result.Match<IActionResult>(
            errors =>
            {
                var error = errors.FirstOrDefault();
                return View("ConfirmEmailError");
            },
             success => View("ConfirmEmailSuccess"));
    }
    [HttpGet]
    public IActionResult ResendEmailConfirmation()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM, CancellationToken cancellationToken)
    {
        var result = await _authRepository.ResendEmailConfirmationAsync(resendEmailConfirmationVM, cancellationToken);

        return result.Match<IActionResult>(
            errors =>
            {
                var error = errors.FirstOrDefault();
                return View("ConfirmEmailError");
            },
             success => View("SuccesfulResendEmailConfirmation"));
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM, CancellationToken cancellationToken)
    {
        var result = await _authRepository.ForgetPasswordAsync(forgotPasswordVM, cancellationToken);

        return result.Match<IActionResult>(
            errors =>
            {
                var error = errors.FirstOrDefault();
                return View("ForgotPasswordError");
            },
             success => View("ForgotPasswordSuccess"));
    }
}
