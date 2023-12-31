var builder = WebApplication.CreateBuilder(args);

builder
    .AddCorsServices()
    .AddFastEndpointsServices()
    .AddIdentityServices()
    .AddMailServices()
    .AddDbServices();

var app = builder.Build();

app
    .AddCors()
    .AddIdentityMiddleware()
    .AddFastEndpointsMiddleware();
app
    .AddGeneratedClientEndpoints();
app
    .AddIdentityData();

app.Run();

namespace Gears.Host
{
    public sealed class Program;
}