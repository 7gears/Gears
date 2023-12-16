var builder = WebApplication.CreateBuilder(args);

builder
    .ConfigureFastEndpointsServices()
    .ConfigureIdentityServices()
    .ConfigureDbServices()
    .ConfigureSwaggerServices();

var app = builder.Build();

app
    .ConfigureIdentityMiddleware()
    .ConfigureFastEndpointsMiddleware()
    .ConfigureSwaggerMiddleware();

app
    .ConfigureIdentityData();

app.Run();