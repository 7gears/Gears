namespace Host.Endpoints;

internal sealed class CorsSettings
{
    public string AllowedOrigins { get; init; }
    public string AllowedMethods { get; init; }
    public string AllowedHeaders { get; init; }
    public string ExposedHeaders { get; init; }
    public bool AllowCredentials { get; init; }
    public int PreflightMaxAgeInSeconds { get; init; }
}

internal static class CorsConfiguration
{
    public static WebApplicationBuilder AddCorsServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<CorsSettings>(builder.Configuration.GetSection("Cors"))
            .AddCors();

        return builder;
    }

    public static IApplicationBuilder AddCors(this IApplicationBuilder builder)
    {
        var options = builder.ApplicationServices.GetRequiredService<IOptions<CorsSettings>>();

        builder.UseCors(CorsPolicyBuilder(options.Value));

        return builder;
    }

    private static Action<CorsPolicyBuilder> CorsPolicyBuilder(CorsSettings settings) =>
        builder =>
            {
                builder
                    .WithOrigins(settings.AllowedOrigins.Split(','))
                    .WithMethods(settings.AllowedMethods.Split(','))
                    .WithHeaders(settings.AllowedHeaders.Split(','))
                    .WithExposedHeaders(settings.ExposedHeaders.Split(','))
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(settings.PreflightMaxAgeInSeconds));

                if (!settings.AllowedOrigins.Equals("*"))
                {
                    if (settings.AllowCredentials)
                    {
                        builder.AllowCredentials();
                    }
                    else
                    {
                        builder.DisallowCredentials();
                    }
                }
            };
}
