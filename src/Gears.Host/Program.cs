using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

ConfigureSettings();
ConfigureServices();

var app = builder.Build();

ConfigureMiddleware();

app.Run();

void ConfigureSettings()
{
    builder
        .ConfigureDbSettings()
        .ConfigureSwaggerSettings();
}

void ConfigureServices()
{
    builder.Services
        .AddFastEndpoints(x =>
        {
            x.DisableAutoDiscovery = true;
            x.Assemblies = new[] { typeof(Gears.Application.Features.Users.GetAllUsersEndpoint).Assembly };
        })
        .ConfigureIdentityServices()
        .ConfigureDbServices()
        .ConfigureSwaggerServices();
}

void ConfigureMiddleware()
{
    app
        .UseFastEndpoints()
        .ConfigureSwaggerMiddleware();
}