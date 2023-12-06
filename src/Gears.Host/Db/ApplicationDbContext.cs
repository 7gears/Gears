namespace Gears.Host.Db;

public sealed class ApplicationDbContext : IdentityDbContext<User, Role, string>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}