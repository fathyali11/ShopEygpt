namespace Web.DataAccess.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Wishlist> Wishlist { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    public DbSet<ProductRating> ProductRatings { get; set; }
    public DbSet<UserRecommendation> UserRecommendations { get; set; }
}
