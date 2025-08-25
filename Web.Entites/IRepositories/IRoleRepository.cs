namespace Web.Entites.IRepositories;
public interface IRoleRepository
{
    Task<IEnumerable<SelectListItem>> GetRoleSelectListAsync(CancellationToken cancellationToken=default);
}
