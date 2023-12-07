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

    public static WebApplicationBuilder ConfigureDbServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var dbOptions = sp.GetRequiredService<IOptions<DbSettings>>();
                options.UseSqlServer(dbOptions.Value.ConnectionString);
            });

        builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return builder;
    }
}