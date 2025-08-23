namespace WearUp.Web.Controllers
{
    [Authorize(Roles =$"{UserRoles.Admin},{UserRoles.Customer}")]
    public class UsersController(IAuthRepository _authRepository,
        IApplicaionUserRepository _applicaionUserRepository) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index(FilterRequest request,CancellationToken cancellationToken)
        {
            var users = await _applicaionUserRepository.GetAllUsersAsync(request,cancellationToken);
            ViewData["SearchTerm"] = request.SearchTerm;
            ViewData["SortField"] = request.SortField;
            ViewData["SortOrder"] = request.SortOrder;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(string id,CancellationToken cancellationToken)
        {
            var isUpdate=await _applicaionUserRepository.ToggleUserAsync(id,cancellationToken);
            return isUpdate ?
                Json(new { success = true, message = "user toggled successfully" }) :
                Json(new { success = false, message = "user doesn't toggle" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id,CancellationToken cancellationToken)
        {
            var isDelete=await _applicaionUserRepository.DeleteUserAsync(id,cancellationToken);
            return isDelete ?
                Json(new { success = true, message = "user deleted successfully" }) :
                Json(new { success = false, message = "user doesn't delete" });
        }

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
