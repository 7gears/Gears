namespace Application.Infrastructure;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }

    DbSet<Role> Roles { get; set; }
}