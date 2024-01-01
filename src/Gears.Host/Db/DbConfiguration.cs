namespace Gears.Host.Db;

internal sealed class DbSettings
{
    public string ConnectionString { get; init; }
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

    public static IApplicationBuilder AddDbData(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        return builder;
    }
}
