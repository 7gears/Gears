namespace Gears.Host.Configuration.Db;

internal sealed class DbSettings
{
    public string ConnectionString { get; init; }
}

internal static class DbConfiguration
{
    private const string MsSqlMigrationAssembly = "Gears.MsSql";

    public static WebApplicationBuilder ConfigureDbSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Db"));

        return builder;
    }

    public static WebApplicationBuilder ConfigureDbServices(this WebApplicationBuilder builder)
    {
        using var sp = builder.Services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<DbSettings>>();
        var connectionString = options.Value.ConnectionString;

        builder.Services.AddDbContext<ApplicationDbContext>(
            x => x.UseSqlServer(connectionString, y => y.MigrationsAssembly(MsSqlMigrationAssembly)));

        return builder;
    }
}