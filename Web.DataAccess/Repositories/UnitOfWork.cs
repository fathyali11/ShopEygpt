

namespace Web.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository CategoryRepository{ get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IShoppingCartRepository ShoppingCartRepository { get; private set; }
        public IOrderHeaderReposittory OrderHeaderReposittory {  get; private set; }
        public IOrderDetailReposittory OrderDetailReposittory{ get; private set; }
        public IApplicaionUserRepository  ApplicaionUserRepository {  get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            this._context = context;
            CategoryRepository=new CategoryRepository(_context);
            ProductRepository = new ProductRepository(_context);
            ShoppingCartRepository = new ShoppingCartRepository(_context);
            OrderHeaderReposittory=new OrderHeaderReposittory(_context);
            OrderDetailReposittory= new OrderDetailReposittory(_context);
            ApplicaionUserRepository=new ApplicationUserRepository(_context);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
