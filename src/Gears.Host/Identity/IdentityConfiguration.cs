namespace Gears.Host.Identity;

internal static class IdentityConfiguration
{
    private const string EmailConfirmationTokenProviderName = nameof(EmailConfirmationTokenProviderName);
    private const string PasswordResetTokenProviderName = nameof(PasswordResetTokenProviderName);

    public static WebApplicationBuilder AddIdentityServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentityCore<User>(x =>
            {
                x.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProviderName;
                x.Tokens.PasswordResetTokenProvider = PasswordResetTokenProviderName;

                x.User.RequireUniqueEmail = true;
                x.Password.RequiredLength = 8;
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddTokenProvider<EmailConfirmationTokenProvider>(EmailConfirmationTokenProviderName)
            .AddTokenProvider<PasswordResetTokenProvider>(PasswordResetTokenProviderName);

        builder.Services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromDays(3));

        builder.Services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromDays(1));

        builder.Services
            .AddAuthorization();

        return builder;
    }

    public static IApplicationBuilder AddIdentity(this IApplicationBuilder builder) =>
        builder
            .UseAuthentication()
            .UseAuthorization();

    public static IApplicationBuilder AddIdentityData(this IApplicationBuilder builder)
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
