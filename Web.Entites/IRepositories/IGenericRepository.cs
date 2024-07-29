
using System.Linq.Expressions;

namespace Web.Entites.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T,bool>>?filter=null,string ?includeObj=null);
        T GetBy(Expression<Func<T, bool>>? filter = null, string? includeObj = null);
        void Add(T entity);
        void Remove(T entity);
    }
}
