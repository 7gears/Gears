﻿namespace IntegrationTests.Infrastructure;

public class InMemoryFixture : TestFixture<Host.Program>
{
    private SqliteConnection _connection;

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        builder.UseSetting("Db:UseMigrations", false.ToString());
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        _connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

        services.Remove(typeof(DbContextOptions<ApplicationDbContext>));
        services.Remove(typeof(DbConnection));
        services.Remove(typeof(IMailService));

        services.AddSingleton(_ => A.Fake<IMailService>());
        services.AddDbContext<ApplicationDbContext>(
            context => context
                .UseLoggerFactory(new NullLoggerFactory())
                .UseSqlite(_connection)
                .ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning))
        );

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseLoggerFactory(new NullLoggerFactory())
            .UseSqlite(_connection!)
            .Options;

        var context = new ApplicationDbContext(options);
        _connection!.Open();
        context.Database.EnsureCreated();
    }

    protected override Task TearDownAsync()
    {
        _connection.Dispose();

        return Task.CompletedTask;
    }
}
