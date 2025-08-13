namespace WearUp.Web.Controllers
{
    [Authorize(Roles =$"{UserRoles.Admin},{UserRoles.Customer}")]
    public class UsersController(IAuthRepository _authRepository) : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterVM model,CancellationToken cancellationToken)
        {
            var result=await _authRepository.RegisterAsync(model, cancellationToken);
            if (result.IsT1)
                return RedirectToAction(nameof(Index));
            result.AsT0.ForEach(error =>
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            });

            return View(model);
        }


    }
}
