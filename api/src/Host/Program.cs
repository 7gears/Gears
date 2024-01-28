using Host.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddCorsServices()
    .AddFastEndpointsServices()
    .AddSwaggerServices()
    .AddIdentityServices()
    .AddMailServices()
    .AddDbServices();

builder.Host.UseDefaultServiceProvider(x =>
{
    x.ValidateOnBuild = true;
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app
    .AddCors()
    .AddIdentity()
    .AddFastEndpoints()
    .AddSwagger();
app
    .AddGeneratedClientEndpoints();
app
    .ApplyMigrations()
    .SeedIdentity();

app.MapFallbackToFile("index.html");

app.Run();

namespace Host
{
    public sealed class Program
    {
    }
}
