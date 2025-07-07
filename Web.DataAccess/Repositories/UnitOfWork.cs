namespace Web.DataAccess.Repositories;
public class UnitOfWork(ApplicationDbContext _context) : IUnitOfWork
{
    public ICategoryRepository CategoryRepository{ get; private set; }= new CategoryRepository(_context);
    public IProductRepository ProductRepository { get; private set; }= new ProductRepository(_context);
    public IShoppingCartRepository ShoppingCartRepository { get; private set; }= new ShoppingCartRepository(_context);
    public IOrderHeaderReposittory OrderHeaderReposittory {  get; private set; }= new OrderHeaderReposittory(_context);
    public IOrderDetailReposittory OrderDetailReposittory{ get; private set; }= new OrderDetailReposittory(_context);
    public IApplicaionUserRepository  ApplicaionUserRepository {  get; private set; }= new ApplicationUserRepository(_context);

    public async Task<int> SaveAsync()
        => await _context.SaveChangesAsync();
}

