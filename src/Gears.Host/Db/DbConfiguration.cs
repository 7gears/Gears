namespace Gears.Host.Db;

internal sealed class DbSettings
{
    public string ConnectionString { get; init; }
}

internal static class DbConfiguration
{
    public static WebApplicationBuilder ConfigureDbSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Db"));

        return builder;
    }

    public static IServiceCollection ConfigureDbServices(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var dbOptions = sp.GetRequiredService<IOptions<DbSettings>>();
                options.UseSqlServer(dbOptions.Value.ConnectionString);
            });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}