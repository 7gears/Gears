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

namespace Host
{
    public sealed class Program
    {
    }
}
