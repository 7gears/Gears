namespace Gears.Host.Identity;

internal static class IdentityConfiguration
{
    public static WebApplicationBuilder ConfigureIdentityServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services
            .AddAuthorization();

        return builder;
    }

    public static IApplicationBuilder ConfigureIdentityMiddleware(this IApplicationBuilder builder) =>
        builder
            .UseAuthentication()
            .UseAuthorization();

    public static IApplicationBuilder ConfigureIdentityData(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        SeedIdentity(context);

        return builder;
    }

    private static void SeedIdentity(ApplicationDbContext context)
    {
        var rootRole = context.Roles.FirstOrDefault(x => x.NormalizedName == "ROOT");
        if (rootRole != null)
        {
            return;
        }

        rootRole = new Role
        {
            Name = "root",
            NormalizedName = "ROOT"
        };
        var rootUser = new User
        {
            UserName = "root",
            NormalizedUserName = "ROOT",
            Email = "root@root",
            NormalizedEmail = "ROOT@ROOT",
            EmailConfirmed = true
        };

        PasswordHasher<User> hasher = new();
        var hash = hasher.HashPassword(rootUser, "Password123!");
        rootUser.PasswordHash = hash;

        context.Roles.Add(rootRole);
        context.Users.Add(rootUser);
        context.SaveChanges();

        context.UserRoles.Add(new IdentityUserRole<string>
        {
            RoleId = rootRole.Id,
            UserId = rootUser.Id
        });
        context.SaveChanges();
    }
}