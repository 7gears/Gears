﻿using FastEndpoints.Testing;
using Gears.Host.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Gears.IntegrationTests.Infrastructure;

public sealed class InMemoryFixture : TestFixture<Host.Program>
{
    private SqliteConnection _connection;

    protected override void ConfigureServices(IServiceCollection services)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        _connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

        services.Remove(services.SingleOrDefault(service => typeof(DbContextOptions<ApplicationDbContext>) == service.ServiceType));
        services.Remove(services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType));

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