namespace WearUp.Web.Controllers;
public class RolesController(IRoleRepository _roleRepository) : Controller
{
    [HttpGet]
    public async Task<IActionResult> LoadRolesSelectList()
    {
        var roles = await _roleRepository.GetRoleSelectListAsync();
        return PartialView("_RolesSelectListPartial", roles);
    }
}
