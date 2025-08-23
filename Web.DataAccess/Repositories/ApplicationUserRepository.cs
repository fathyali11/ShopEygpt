namespace Web.DataAccess.Repositories;
public class ApplicationUserRepository(ApplicationDbContext _context,
    HybridCache _hybridCache) :  IApplicaionUserRepository
{

    public async Task<PaginatedList<UserResponseForAdmin>> GetAllUsersAsync(FilterRequest request, CancellationToken cancellationToken=default)
	{
        var cacheKey = $"{UserCacheKeys.AllUsersForAdmin}" +
            $"_{request.SearchTerm}_{request.SortField}" +
            $"_{request.SortOrder}_{request.PageNumber}";

        var users=await _hybridCache.GetOrCreateAsync(cacheKey,
            async _ =>
    {
                var query =_context.Users.AsNoTracking().
                ProjectToType<UserResponseForAdmin>();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                    query = query.Where(u =>
                    u.UserName.Contains(request.SearchTerm) ||
                    u.Email.Contains(request.SearchTerm) ||
                    u.FirstName.Contains(request.SearchTerm) ||
                    u.LastName.Contains(request.SearchTerm));

                query = request.SortField?.ToLower() switch
                {
                    "username" => request.SortOrder?.ToLower() == "asc" ?
                        query.OrderBy(u => u.UserName) :
                        query.OrderByDescending(u => u.UserName),

                    "email" => request.SortOrder?.ToLower() == "asc" ?
                        query.OrderBy(u => u.Email) :
                        query.OrderByDescending(u => u.Email),

                    "firstname" => request.SortOrder?.ToLower() == "asc" ?
                        query.OrderBy(u => u.FirstName) :
                        query.OrderByDescending(u => u.FirstName),

                    "lastname" => request.SortOrder?.ToLower() == "asc" ?
                        query.OrderBy(u => u.LastName) :
                        query.OrderByDescending(u => u.LastName),

                    _ => query.OrderBy(u => u.UserName)
                };

                return await query.ToListAsync(cancellationToken);
            },
            tags: [UserCacheKeys.UsersTag],
            cancellationToken:cancellationToken); 

        return PaginatedList<UserResponseForAdmin>.Create(users, request.PageNumber, PaginationConstants.DefaultPageSize);
    }
    public void Update(ApplicationUser user)
    {
        var oldUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == user.Id);
        if (oldUser != null)
        {
            //oldUser.Name = user.Name;
            //oldUser.Email = user.Email;
            //oldUser.Phone = user.Phone;
            //oldUser.City = user.City;
        }
    }

}
