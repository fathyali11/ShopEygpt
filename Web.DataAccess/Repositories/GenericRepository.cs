using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Web.DataAccess.Data;
using Web.Entites.IRepositories;

namespace Web.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeObj = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            if(filter != null)
            {
                query = query.Where(filter);
            }
            if(includeObj != null)
            {
                foreach(var item in includeObj.Split(','))
                {
                    query=query.Include(item);
                }
            }
            return query;
        }

        public T GetBy(Expression<Func<T, bool>>? filter = null, string? includeObj = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeObj != null)
            {
                foreach (var item in includeObj.Split(','))
                {
                    query = query.Include(item);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
