namespace  Web.DataAccess.Repositories.Tests;
public class RoleRepositoryTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
    private RoleRepository CreateRepository(ApplicationDbContext context)
    {
        return new RoleRepository(context);
    }
    [Fact()]
    public async Task GetRoleSelectListAsyncTest()
    {
        var context = CreateContext();

        context.Roles.AddRange(
            [
            new IdentityRole { Id="admin", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id="user",  Name = "User", NormalizedName = "USER" } ,
            new IdentityRole { Id="manager",  Name = "Manager", NormalizedName = "MANAGER" },
            new IdentityRole { Id="guest",  Name = "Guest", NormalizedName = "GUEST" }
            ]
            );
        context.SaveChanges();

        var repository = CreateRepository(context);

        // Act
        var result= await repository.GetRoleSelectListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().BeInAscendingOrder(r => r.Text);
    }
}