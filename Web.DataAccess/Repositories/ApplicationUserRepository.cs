namespace Web.DataAccess.Repositories;
public class ApplicationUserRepository(ApplicationDbContext _context,
    UserManager<ApplicationUser> _userManager,
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
                var query =(from user in _context.Users
                           join userRole in _context.UserRoles
                           on user.Id equals userRole.UserId
                           join role in _context.Roles
                           on userRole.RoleId equals role.Id
                           select new UserResponseForAdmin
                           {
                               Id = user.Id,
                               UserName = user.UserName!,
                               Email = user.Email!,
                               FirstName = user.FirstName!,
                               LastName = user.LastName!,
                               IsActive = user.IsActive,
                               Role = role.Name!
                           });

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
                    
                    "role" => request.SortOrder?.ToLower() == "asc" ?
                        query.OrderBy(u => u.Role) :
                        query.OrderByDescending(u => u.Role),

                    _ => query.OrderBy(u => u.UserName)
                };

                return await query.ToListAsync(cancellationToken);
            },
            tags: [UserCacheKeys.UsersTag],
            cancellationToken:cancellationToken); 

        return PaginatedList<UserResponseForAdmin>.Create(users, request.PageNumber, PaginationConstants.DefaultPageSize);
    }

    public async Task<OneOf<bool,ValidationError>> CreateUserAsync(CreateUserVM model,CancellationToken cancellationToken=default)
    {
        var existingUser= await _context.Users
            .AnyAsync(x => x.UserName == model.UserName || x.Email == model.Email, cancellationToken);
        if (existingUser)
            return new ValidationError("User Found","UserName or Email already exists.");

        var user =model.Adapt<ApplicationUser>();
        var result= await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) 
            return new ValidationError("Create User Failed",string.Join(",",result.Errors.Select(x=>x.Description)));
        user.EmailConfirmed = true;
        var roleResult= await _userManager.AddToRoleAsync(user, model.Role);
        if (!roleResult.Succeeded) 
            return new ValidationError("Assign Role Failed",string.Join(",",roleResult.Errors.Select(x=>x.Description)));
        
        await _context.SaveChangesAsync(cancellationToken);
        await RemoveCacheKey(cancellationToken);
        return true;
    }
    public async Task<bool> ToggleUserAsync(string id,CancellationToken cancellationToken=default)
    {
        var isUpdated= await _context.Users
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.IsActive, x => !x.IsActive));
        if(isUpdated>0)
        {
            await RemoveCacheKey(cancellationToken);
            return true;
        }
        return false;
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
        
        if(isDeleted>0)
        {
            await RemoveCacheKey(cancellationToken);
            return true;
        }
        return false;
    }

    public async Task<UserProfileVM> GetUserProfileAsync(string userId,CancellationToken cancellationToken=default)
    {
        var user= await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .ProjectToType<UserProfileVM>()
            .FirstAsync(cancellationToken);
        return user;
    }
    public async Task RemoveCacheKey(CancellationToken cancellationToken)
    {
        await _hybridCache.RemoveByTagAsync(UserCacheKeys.UsersTag, cancellationToken);
    }
}
