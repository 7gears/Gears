var builder = WebApplication.CreateBuilder(args);

ConfigureServices();

var app = builder.Build();

ConfigureMiddleware();
ConfigureData();

app.Run();

void ConfigureServices() =>
    builder
        .ConfigureFastEndpointsServices()
        .ConfigureIdentityServices()
        .ConfigureDbServices()
        .ConfigureSwaggerServices();

void ConfigureMiddleware() =>
    app
        .ConfigureIdentityMiddleware()
        .ConfigureFastEndpointsMiddleware()
        .ConfigureSwaggerMiddleware();

void ConfigureData() =>
    app
        .ConfigureIdentityData();