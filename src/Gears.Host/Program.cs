var builder = WebApplication.CreateBuilder(args);

builder
    .AddCorsServices()
    .AddFastEndpointsServices()
    .AddSwaggerServices()
    .AddIdentityServices()
    .AddMailServices()
    .AddDbServices();

var app = builder.Build();

app
    .AddCors()
    .AddIdentity()
    .AddFastEndpoints()
    .AddSwagger();
app
    .AddGeneratedClientEndpoints();
app
    .AddDbData()
    .AddIdentityData();

app.Run();

namespace Gears.Host
{
    public sealed class Program;
}
