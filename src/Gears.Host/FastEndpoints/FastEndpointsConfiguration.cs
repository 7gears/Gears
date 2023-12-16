namespace Gears.Host.FastEndpoints;

internal static class FastEndpointsConfiguration
{
    public static WebApplicationBuilder ConfigureFastEndpointsServices(this WebApplicationBuilder builder)
    {
        var key = builder.Configuration.GetValue<string>($"Jwt:{nameof(JwtConfiguration.Key)}");

        builder.Services
            .Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"))
            .AddFastEndpoints(x =>
            {
                x.DisableAutoDiscovery = true;
                x.Assemblies = new[] { typeof(Application.Features.Users.GetAllUsersEndpoint).Assembly };
            })
            .AddJWTBearerAuth(key)
            .RegisterServicesFromGearsHost();

        return builder;
    }

    public static IApplicationBuilder ConfigureFastEndpointsMiddleware(this IApplicationBuilder builder) =>
        builder.UseFastEndpoints(x =>
        {
            x.Errors.ProducesMetadataType = typeof(ProblemDetails);
            x.Errors.UseProblemDetails();
        });
}