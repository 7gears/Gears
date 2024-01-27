namespace Host.Db;

internal static class DbMigrator
{
    public static WebApplication ApplyMigrations(this WebApplication app)
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
