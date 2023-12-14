namespace Gears.Host.Identity;

internal static class IdentityConfiguration
{
    public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services)
    {
        services
            .AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }
}