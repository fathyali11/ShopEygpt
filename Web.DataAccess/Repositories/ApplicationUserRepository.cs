using System.Threading;

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

    public async Task<bool> ToggleUserAsync(string id,CancellationToken cancellationToken=default)
    {
        var isUpdated= await _context.Users
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.IsActive, x => !x.IsActive));
        await RemoveCacheKey(cancellationToken);
        return isUpdated>0?true:false;
    }

    public async Task<EditUserVM> GetUserForEditAsync(string id,CancellationToken cancellationToken=default)
    {
        var user= await _context.Users
            .AsNoTracking()
            .ProjectToType<EditUserVM>()
            .FirstAsync(x => x.Id == id, cancellationToken);

        var roleName=await (from userRole in _context.UserRoles
                          join role in _context.Roles
                          on userRole.RoleId equals role.Id
                          where userRole.UserId == id
                          select role.Name).FirstOrDefaultAsync(cancellationToken);

        user.Role = roleName!;
        return user;
    }

    public async Task<bool> UpdateUserAsync(EditUserVM model,CancellationToken cancellationToken=default)
    {
        var user=_context.Users.FirstOrDefault(x=>x.Id==model.Id);
        if(user is null) return false;

        model.Adapt(user);

        var userRoles = await _context.UserRoles
                .Where(x => x.UserId == user.Id)
                .ToListAsync(cancellationToken);
        if (userRoles.Any())
            _context.UserRoles.RemoveRange(userRoles);

        var role =await _context.Roles.FirstOrDefaultAsync(x => x.Name == model.Role,cancellationToken);
        if (role is null)
            return false;

        await _context.UserRoles.AddAsync(new()
        {
            UserId = user.Id,
            RoleId = role.Id
        }, cancellationToken);

       
        await RemoveCacheKey(cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<bool> DeleteUserAsync(string id,CancellationToken cancellationToken=default)
    {
        var isDeleted= await _context.Users
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        await RemoveCacheKey(cancellationToken);
        return isDeleted>0?true:false;
    }

    private async Task RemoveCacheKey(CancellationToken cancellationToken)
    {
        await _hybridCache.RemoveByTagAsync(UserCacheKeys.UsersTag, cancellationToken);
    }
}
