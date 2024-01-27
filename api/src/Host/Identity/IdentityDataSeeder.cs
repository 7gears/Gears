namespace Host.Identity;

internal static class IdentityDataSeeder
{
    public static readonly string RootRoleName = "Root";
    public static readonly string RootUserEmail = "root@root";
    public static readonly string RootPassword = "123456";

    public static WebApplication SeedIdentity(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<DbSettings>>();

        if (options.Value.UseSeedData)
        {
            SeedIdentity(scope);
        }

        return app;
    }

    private static void SeedIdentity(IServiceScope scope)
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        var rootRole = context.Roles.FirstOrDefault(x => x.NormalizedName == RootRoleName.ToUpper());
        if (rootRole != null)
        {
            return;
        }

        // 1# Root Role
        rootRole = new Role
        {
            Name = RootRoleName,
            NormalizedName = RootRoleName.ToUpper(),
            IsDefault = false
        };

        // 2# Root User
        var rootUser = new User
        {
            Email = RootUserEmail,
            NormalizedEmail = RootUserEmail.ToUpper(),
            EmailConfirmed = true,
            IsActive = true
        };

        PasswordHasher<User> hasher = new();
        var hash = hasher.HashPassword(rootUser, RootPassword);
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

        // 3# Add all permissions to Root role
        foreach (var permission in Allow.AllNames())
        {
            roleManager.AddClaimAsync(rootRole, ToClaim(permission))
                .GetAwaiter()
                .GetResult();
        }
    }

    private static Claim ToClaim(string permission) =>
        new(Consts.Auth.PermissionClaimType, permission);
}
