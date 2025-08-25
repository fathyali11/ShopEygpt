namespace WearUp.Web.Controllers;
public class UsersController(IApplicaionUserRepository _applicaionUserRepository) : Controller
{
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> Index(FilterRequest request,CancellationToken cancellationToken)
    {
        var users = await _applicaionUserRepository.GetAllUsersAsync(request,cancellationToken);
        ViewData["SearchTerm"] = request.SearchTerm;
        ViewData["SortField"] = request.SortField;
        ViewData["SortOrder"] = request.SortOrder;
        return View(users);
    }
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user=await _applicaionUserRepository.GetUserForEditAsync(id);
        return View(user);
    }
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserVM model,CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);
        var isUpdate=await _applicaionUserRepository.UpdateUserAsync(model,cancellationToken);
        if (isUpdate)
            return RedirectToAction(nameof(Index));
        ModelState.AddModelError(string.Empty, "Failed to update user");
        return View(model);
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
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id,CancellationToken cancellationToken)
    {
        var isDelete=await _applicaionUserRepository.DeleteUserAsync(id,cancellationToken);
        return isDelete ?
            Json(new { success = true, message = "user deleted successfully" }) :
            Json(new { success = false, message = "user doesn't delete" });
    }

    [Authorize(Roles =UserRoles.Admin)]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserVM model,CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);
        var isCreate=await _applicaionUserRepository.CreateUserAsync(model,cancellationToken);
        if (isCreate.IsT0)
            return RedirectToAction(nameof(Index));
        ModelState.AddModelError(isCreate.AsT1.PropertyName,isCreate.AsT1.ErrorMessage);
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile=await _applicaionUserRepository.GetUserProfileAsync(userId!,cancellationToken);
        return View(userProfile);
    }


}
