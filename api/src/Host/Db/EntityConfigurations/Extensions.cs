namespace Host.Db.EntityConfigurations;

public static class ModelBuilderExtensions
{
    public static void ApplyCommonConfigurations(this ModelBuilder builder)
    {
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
