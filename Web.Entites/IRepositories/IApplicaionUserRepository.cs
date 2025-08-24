namespace Web.Entites.IRepositories;
public interface IApplicaionUserRepository
{
    Task<PaginatedList<UserResponseForAdmin>> GetAllUsersAsync(FilterRequest request, CancellationToken cancellationToken = default);
    Task<bool> ToggleUserAsync(string id, CancellationToken cancellationToken = default);
    Task<EditUserVM> GetUserForEditAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(EditUserVM model, CancellationToken cancellationToken = default);






}
