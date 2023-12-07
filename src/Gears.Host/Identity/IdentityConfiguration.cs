namespace Gears.Host.Identity;

internal static class IdentityConfiguration
{
    public static WebApplicationBuilder ConfigureIdentityServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return builder;
    }
}