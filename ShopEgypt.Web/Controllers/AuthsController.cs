using Web.Entites.ViewModels.UsersVMs;

namespace ShopEgypt.Web.Controllers;
public class AuthsController(IAuthRepository _authRepository) : Controller
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
    public IActionResult Login() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM loginVM, CancellationToken cancellationToken)
    {
        var result = await _authRepository.LoginAsync(loginVM, cancellationToken);
        if (result.IsT1)
            return RedirectToAction("Index", "Home");

        var errors = result.AsT0;
        foreach (var item in errors)
            ModelState.AddModelError(item.PropertyName, item.ErrorMessage);

        return View(loginVM);

    }
}
