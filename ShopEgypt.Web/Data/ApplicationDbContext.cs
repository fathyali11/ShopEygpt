using Microsoft.EntityFrameworkCore;
using ShopEgypt.Web.Models;

namespace ShopEgypt.Web.Data
{
	public class ApplicationDbContext:DbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
    }

}
