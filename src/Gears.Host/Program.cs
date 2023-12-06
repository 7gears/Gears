using Gears.Host.Configuration.Db;

var builder = WebApplication.CreateBuilder(args);

ConfigureConfiguration();
ConfigureServices();

var app = builder.Build();

ConfigureMiddleware();
ConfigureEndpoints();

app.Run();

void ConfigureConfiguration()
{
    builder.ConfigureDbSettings();
    builder.ConfigureSwaggerSettings();
}

void ConfigureServices()
{
    builder.ConfigureDbServices();
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