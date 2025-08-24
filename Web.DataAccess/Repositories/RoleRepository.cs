namespace Web.DataAccess.Repositories;
public class RoleRepository(ApplicationDbContext _context):IRoleRepository
{
    public async Task<IEnumerable<SelectListItem>> GetRoleSelectListAsync(CancellationToken cancellationToken=default)
    {
        var roles = await _context.Roles
            .OrderBy(r => r.Name)
            .Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            })
            .ToListAsync(cancellationToken);
        return roles;
    }
}
