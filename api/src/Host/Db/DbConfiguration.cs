namespace Host.Db;

internal sealed class DbSettings
{
    public string ConnectionString { get; init; }

    public bool UseMigrations { get; init; }

    public bool UseSeedData { get; init; }
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
}
