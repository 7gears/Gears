namespace Gears.Host.Db;

internal sealed class DbSettings
{
    public string ConnectionString { get; init; }

    public bool UseMigrations { get; init; }
}

internal static class DbConfiguration
{
    public static WebApplicationBuilder AddDbServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<DbSettings>(builder.Configuration.GetSection("Db"))
            .AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var dbOptions = sp.GetRequiredService<IOptions<DbSettings>>();
                options.UseSqlServer(dbOptions.Value.ConnectionString);
            });

        return builder;
    }

    public static WebApplication AddDbData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<DbSettings>>();

        if (options.Value.UseMigrations && context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        return app;
    }
}
