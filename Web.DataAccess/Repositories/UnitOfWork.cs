using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.DataAccess.Data;
using Web.Entites.IRepositories;

namespace Web.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository CategoryRepository{ get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            this._context = context;
            CategoryRepository=new CategoryRepository(_context);
            ProductRepository = new ProductRepository(_context);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
