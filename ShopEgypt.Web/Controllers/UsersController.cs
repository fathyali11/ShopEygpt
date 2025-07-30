using System.Runtime.CompilerServices;
using Web.Entites.ViewModels.UsersVMs;

namespace ShopEgypt.Web.Controllers
{
    [Authorize(Roles =$"{UserRoles.Admin},{UserRoles.Customer}")]
    public class UsersController(IAuthRepository _authRepository) : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterVM request,CancellationToken cancellationToken)
        {
            var result=await _authRepository.RegisterAsync(request,cancellationToken);
            if (result.IsT1)
                return View("Register");

            result.AsT0.ForEach(error =>
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            });

            return View("Register", request);
        }

    }
}
