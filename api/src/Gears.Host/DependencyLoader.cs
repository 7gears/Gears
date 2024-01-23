namespace Gears.Host;

public static class DependencyLoader
{
    public static IServiceCollection LoadServices(this IServiceCollection services)
    {
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
        services.AddScoped<IHttpContextService, HttpContextService>();

        return services;
    }
}
