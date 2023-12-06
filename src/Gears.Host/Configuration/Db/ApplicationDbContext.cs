namespace Gears.Host.Configuration.Db;

public sealed class ApplicationDbContext : IApplicationDbContext
{
    public DbSet<User> Users { get; }

    public DbSet<Role> Roles { get; }
}