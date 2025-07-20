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
        return RedirectToAction("Login", "Auths");
    }
}
