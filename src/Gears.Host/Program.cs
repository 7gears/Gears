var builder = WebApplication.CreateBuilder(args);

ConfigureSettings();
ConfigureServices();

var app = builder.Build();

ConfigureMiddleware();
ConfigureEndpoints();

app.Run();

void ConfigureSettings()
{
    builder
        .ConfigureDbSettings()
        .ConfigureIdentityServices()
        .ConfigureSwaggerSettings();
}

void ConfigureServices()
{
    builder
        .ConfigureDbServices()
        .ConfigureSwaggerServices();
}

void ConfigureMiddleware()
{
    app.ConfigureSwaggerMiddleware();
}

void ConfigureEndpoints()
{
    app.MapGet("/", (UserManager<User> userManager) =>
    {
        var users = userManager.Users.ToList();

        return "Hello World!";
    });
}