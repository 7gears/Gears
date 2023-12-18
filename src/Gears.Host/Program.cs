var builder = WebApplication.CreateBuilder(args);

builder
    .ConfigureFastEndpointsServices()
    .ConfigureIdentityServices()
    .ConfigureDbServices();

var app = builder.Build();

app
    .ConfigureIdentityMiddleware()
    .ConfigureFastEndpointsMiddleware();

app
    .ConfigureGeneratedClientEndpoints();

app
    .ConfigureIdentityData();

app.Run();