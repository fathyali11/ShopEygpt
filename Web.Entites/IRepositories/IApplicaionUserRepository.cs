namespace Web.Entites.IRepositories;
public interface IApplicaionUserRepository
{
    Task<PaginatedList<UserResponseForAdmin>> GetAllUsersAsync(FilterRequest request, CancellationToken cancellationToken = default);
}
