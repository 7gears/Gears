var builder = WebApplication.CreateBuilder(args);

ConfigureSettings();
ConfigureServices();

var app = builder.Build();

ConfigureMiddleware();
ConfigureEndpoints();

app.Run();

void ConfigureSettings()
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