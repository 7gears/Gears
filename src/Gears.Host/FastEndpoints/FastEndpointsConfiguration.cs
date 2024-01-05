namespace Gears.Host.FastEndpoints;

internal sealed class JwtSettings
{
    public string Key { get; init; }
    public int DurationInSeconds { get; init; }
}

internal static class FastEndpointsConfiguration
{
    public static WebApplicationBuilder AddFastEndpointsServices(this WebApplicationBuilder builder)
    {
        var key = builder.Configuration.GetValue<string>($"Jwt:{nameof(JwtSettings.Key)}");

        builder.Services
            .Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"))
            .AddFastEndpoints(x =>
            {
                x.DisableAutoDiscovery = true;
                x.Assemblies = new[] { typeof(Application.Features.Users.GetAllUsersEndpoint).Assembly };
            })
            .AddJWTBearerAuth(key)
            .RegisterServicesFromGearsHost();

        return builder;
    }

    public static IApplicationBuilder AddFastEndpoints(this IApplicationBuilder builder)
    {
        builder.UseFastEndpoints(x =>
        {
            x.Endpoints.ShortNames = true;
            x.Errors.ProducesMetadataType = typeof(ProblemDetails);
            x.Errors.UseProblemDetails();
        });

        return builder;
    }
}
