namespace Host.Db;

[RegisterService<IApplicationDbContext>(LifeTime.Scoped)]
public sealed class ApplicationDbContext : IdentityDbContext<User, Role, string>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(x =>
        {
            x.Property(c => c.FirstName).HasMaxLength(256);
            x.Property(c => c.LastName).HasMaxLength(256);
            x.ToTable("Users");
        });

        builder.Entity<Role>(x =>
        {
            x.Property(c => c.Description).HasMaxLength(1024);
            x.ToTable("Roles");
        });

        builder.Entity<IdentityRoleClaim<string>>(x =>
        {
            x.ToTable("RoleClaims");
        });

        builder.Entity<IdentityUserRole<string>>(x =>
        {
            x.ToTable("UserRoles");
        });

        builder.Entity<IdentityUserClaim<string>>(x =>
        {
            x.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(x =>
        {
            x.ToTable("UserLogins");
        });

        builder.Entity<IdentityUserToken<string>>(x =>
        {
            x.ToTable("UserTokens");
        });
    }
}
