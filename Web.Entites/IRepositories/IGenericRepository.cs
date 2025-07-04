using System.Linq.Expressions;

namespace Web.Entites.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeObj = null);
        Task<T?> GetByAsync(Expression<Func<T, bool>>? filter = null, string? includeObj = null);
        Task AddAsync(T entity);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
    }
}
