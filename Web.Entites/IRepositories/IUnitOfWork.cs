namespace Web.Entites.IRepositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IOrderHeaderReposittory OrderHeaderReposittory { get; }
        IOrderDetailReposittory OrderDetailReposittory { get; }
        IApplicaionUserRepository ApplicaionUserRepository { get; }
        Task<int> SaveAsync();
    }
}
