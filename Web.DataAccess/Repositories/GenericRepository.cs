using System.Linq.Expressions;

namespace Web.DataAccess.Repositories
{
    public class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet=context.Set<T>();
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeObj = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeObj != null)
            {
                foreach (var item in includeObj.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item.Trim());
                }
            }
            return await query.ToListAsync();
        }
        public async Task<T?> GetByAsync(Expression<Func<T, bool>>? filter = null, string? includeObj = null)
        {
            var result = await GetAllAsync(filter, includeObj);
            return result.FirstOrDefault();
        }
        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }
    }
}