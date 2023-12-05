using Gears.Host.Configuration;

var builder = WebApplication.CreateBuilder(args);

ConfigureConfiguration();
ConfigureServices();

var app = builder.Build();

ConfigureMiddleware();
ConfigureEndpoints();

app.Run();

void ConfigureConfiguration()
{
    builder.ConfigureSwaggerSettings();
}

void ConfigureServices()
{
    builder.ConfigureSwaggerServices();
}

void ConfigureMiddleware()
{
    app.ConfigureSwaggerMiddleware();
}

void ConfigureEndpoints()
{
    app.MapGet("/", () => "Hello World!");
}