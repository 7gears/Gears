namespace Gears.Host.Configuration.Db;

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
        return builder;
    }

    public static IApplicationBuilder ConfigureDbMiddleware(this IApplicationBuilder app)
    {
        return app;
    }
}