namespace Gears.Host.Db;

[RegisterService<IApplicationDbContext>(LifeTime.Scoped)]
public sealed class ApplicationDbContext : IdentityDbContext<User, Role, string>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}